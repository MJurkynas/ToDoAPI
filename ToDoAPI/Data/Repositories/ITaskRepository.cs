using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Data.Entities;

namespace ToDoAPI.Data.Repositories
{
	public interface ITaskRepository
	{
		Task<List<ToDoTask>> GetTasks();
		Task<List<ToDoTask>> GetTasks(string userId);
		Task<ToDoTask> GetTask(int id);
		Task<bool> CreateTask(ToDoTask task);
		Task<bool> UpdateTask(ToDoTask task);
		Task<bool> DeleteTask(int id);
	}
}
