using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Models
{
	/// <summary>
	/// <inheritdoc cref="OperationResult"/>
	/// <para>
	/// If <see cref="OperationResult.Suceeded"/> is <c>TRUE</c> then <see cref="Token"/> has a value.
	/// </para>
	/// </summary>
	/// 
	public class LoginResult : OperationResult
	{
		/// <summary>
		/// Has a JWT if the operation was successful.
		/// </summary>
		public string Token { get; set; }
		/// <summary>
		/// <inheritdoc cref="OperationResult()"/> Adds the <paramref name="token"/> to <see cref="Token"/>.
		/// </summary>
		/// <param name="token"></param>
		public LoginResult(string token) : base()
		{
			Token = token;
		}
		/// <summary>
		/// <inheritdoc cref="OperationResult(IEnumerable{string})"/>
		/// </summary>
		/// <param name="errors"></param>
		public LoginResult(ICollection<string> errors) : base(errors)
		{

		}
	}
}
