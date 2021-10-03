using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Data.Entities
{
	public class ToDoTask
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Name { get; set; }
		public bool IsCompleted { get; set; }

		public virtual IdentityUser User { get; set; }
	}
}
