using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoAPI.Contracts;
using ToDoAPI.Models;
using ToDoAPI.Settings;

namespace ToDoAPI.Services
{
	public class AccountService : IAccountService
	{
		private readonly ILogger<AccountService> _logger;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly JwtSettings _jwtSettings;
		public AccountService(ILogger<AccountService> logger, UserManager<IdentityUser> userManager, JwtSettings jwtSettings)
		{
			_logger = logger;
			_userManager = userManager;
			_jwtSettings = jwtSettings;
		}
		public async Task<LoginResult> Login(AuthenticationRequest credentials)
		{
			ICollection<string> errors = new List<string>();

			try
			{
				if (ValidateLoginCredentials(credentials))
				{
					IdentityUser user = await _userManager.FindByEmailAsync(credentials.Email);
					if (user != null)
					{
						if(await _userManager.CheckPasswordAsync(user, credentials.Password))
						{
							return new LoginResult(await GenerateJwtToken(user));
						}
						else
						{
							errors.Add("Invalid password.");
						}
					}
					else
					{
						errors.Add("No such user exists.");
					}
				}
			}
			catch (ArgumentNullException)
			{
				errors.Add("An unexpected error has occurred.");
			}
			catch(ArgumentException)
			{
				errors.Add("Please, provide the necessary credentials.");
			}
			catch(Exception e)
			{
				_logger.LogError(e, "An unexpected error has occurred.");
				errors.Add("An unexpected error has occurred.");
			}

			return new LoginResult(errors);
		}
		private bool ValidateLoginCredentials(AuthenticationRequest credentials)
		{
			if (credentials == null)
			{
				throw new ArgumentNullException(nameof(credentials));
			}

			if (string.IsNullOrWhiteSpace(credentials.Email) || string.IsNullOrWhiteSpace(credentials.Password))
			{
				throw new ArgumentException("Necessary credentials were not provided.");
			}

			return true;
		}
		private async Task<string> GenerateJwtToken(IdentityUser user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

			ICollection<Claim> claims = new List<Claim>();
			claims.Add(new Claim("id", user.Id));
			claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

			var roles = await _userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = null,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		public async Task<string> GetPasswordResetToken(string email)
		{
			IdentityUser user = await _userManager.FindByEmailAsync(email);
			if(user == null)
			{
				throw new KeyNotFoundException($"User with {email} was not found.");
			}

			return await _userManager.GeneratePasswordResetTokenAsync(user);
		}

		public async Task<OperationResult> ResetPassword(string email, string passwordResetToken, string newPassword)
		{
			IdentityUser user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				throw new KeyNotFoundException($"User with {email} was not found.");
			}

			IdentityResult resetResult = await _userManager.ResetPasswordAsync(user, passwordResetToken, newPassword);

			return new OperationResult(resetResult.Errors.Select(x => x.Description));
		}
	}
}
