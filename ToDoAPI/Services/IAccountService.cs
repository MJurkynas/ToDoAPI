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
	}
}
