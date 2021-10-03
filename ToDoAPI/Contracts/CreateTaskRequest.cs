using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Contracts
{
	/// <summary>
	/// Represents a task to create or update.
	/// </summary>
	public class CreateTaskRequest
	{
		/// <summary>
		/// Name of the task.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Whether the task is completed.
		/// </summary>
		public bool IsCompleted { get; set; }
	}
}
