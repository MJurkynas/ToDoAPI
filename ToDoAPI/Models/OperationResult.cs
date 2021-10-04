using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Models
{
	/// <summary>
	/// Defines the state of the completed operation. <see cref="Suceeded"/> is <c>TRUE</c>
	/// if there are no <see cref="Errors"/>.
	/// </summary>
	public class OperationResult
	{
		/// <summary>
		/// <c>TRUE</c> if the operation was successful.
		/// </summary> 
		public bool Suceeded { get; set; }
		/// <summary>
		/// Any errors that have occurred during the execution of the operation.
		/// </summary>
		public IEnumerable<string> Errors { get; set; } = new List<string>();
		/// <summary>
		/// Sets <see cref="Suceeded"/> to <c>TRUE</c>.
		/// </summary>
		public OperationResult()
		{
			Suceeded = true;
		}
		/// <summary>
		/// Sets <see cref="Suceeded"/> to <c>TRUE</c> if an empty <paramref name="errors"/> collection was provided, else <c>FALSE</c>. 
		/// Sets the provided <paramref name="errors"/> to <see cref="Errors"/>.
		/// </summary>
		/// <param name="errors"></param>
		public OperationResult(IEnumerable<string> errors)
		{
			Errors = errors;
			if (Errors.Any())
			{
				Suceeded = false;
			}
		}
	}
}
