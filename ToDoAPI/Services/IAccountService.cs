using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Contracts;
using ToDoAPI.Models;

namespace ToDoAPI.Services
{
	public interface IAccountService
	{
		Task<LoginResult> Login(AuthenticationRequest credentials);
		Task<string> GetPasswordResetToken(string email);
		Task<OperationResult> ResetPassword(string email, string passwordResetToken, string newPassword);
	}
}
