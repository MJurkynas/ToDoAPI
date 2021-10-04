using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Contracts;
using ToDoAPI.Models;
using ToDoAPI.Services;
using Microsoft.AspNetCore.Http.Extensions;

namespace ToDoAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly IAccountService _accountService;
		private readonly IMailService _mailService;

		public AccountController(IAccountService accountService, IMailService mailService)
		{
			_accountService = accountService;
			_mailService = mailService;
		}

		/// <summary>
		/// Returns a JWT if provided with valid user's credentials.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Authenticate")]
		public async Task<LoginResult> Authenticate(AuthenticationRequest request)
		{
			return await _accountService.Login(request);
		}

		/// <summary>
		/// Sends an email with a password reset link.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] string email)
		{
			if (!string.IsNullOrWhiteSpace(email) && MailboxAddress.TryParse(email, out MailboxAddress address))
			{
				try
				{
					string passwordResetToken = await _accountService.GetPasswordResetToken(email);
					await _mailService.SendEmail(new Email
					{
						To = address,
						Subject = "Password reset",
						Body = new Uri(new Uri(Request.GetDisplayUrl()), Url.Action("ResetPassword", "Account", new { token = email + "|" + passwordResetToken })).ToString()
					});
					return Ok();
				}
				catch (KeyNotFoundException)
				{
					return Unauthorized();
				}
			}
			else
			{
				return BadRequest("Invalid email");
			}
		}

		/// <summary>
		/// Resets the user's password to the new password provided.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="newPassword"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("ResetPassword")]
		public async Task<IActionResult> ResetPassword([FromQuery]string token, [FromBody]string newPassword)
		{
			if(!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(newPassword))
			{
				string[] splitToken = token.Split("|");
				if (splitToken.Length == 2)
				{
					try
					{
						OperationResult operationResult = await _accountService.ResetPassword(splitToken[0], splitToken[1], newPassword);
						if (operationResult.Suceeded)
						{
							return Ok();
						}
						else
						{
							return BadRequest(operationResult);
						}
					}
					catch (KeyNotFoundException)
					{
						return Unauthorized();
					}
				}
				else
				{
					return BadRequest("Invalid token");
				}
			}
			else
			{
				return BadRequest("Please, provide a token and a new password");
			}
		}
	}
}
