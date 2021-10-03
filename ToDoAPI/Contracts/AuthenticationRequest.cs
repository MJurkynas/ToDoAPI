using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Contracts
{
	/// <summary>
	/// Has the necessary credentials for authentication.
	/// </summary>
	public class AuthenticationRequest
	{
		/// <summary>
		/// User's email.
		/// </summary>
		public string Email { get; set; }
		/// <summary>
		/// User's password.
		/// </summary>
		public string Password { get; set; }
	}
}
