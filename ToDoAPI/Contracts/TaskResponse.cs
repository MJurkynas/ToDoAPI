using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Contracts
{
	/// <summary>
	/// Represents a task.
	/// </summary>
	public class TaskResponse
	{
		/// <summary>
		/// Id of the task.
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Task owner's id.
		/// </summary>
		public string UserId { get; set; }
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
