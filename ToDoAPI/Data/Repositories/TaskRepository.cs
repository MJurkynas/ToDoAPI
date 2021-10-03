using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Data.Entities;

namespace ToDoAPI.Data.Repositories
{
	public class TaskRepository : ITaskRepository
	{
		private readonly DataContext _dataContext;
		public TaskRepository(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
		public async Task<bool> CreateTask(ToDoTask task)
		{
			_dataContext.ToDoTasks.Add(task);
			return (await _dataContext.SaveChangesAsync()) == 1;
		}

		public async Task<bool> DeleteTask(int id)
		{
			ToDoTask task = await _dataContext.ToDoTasks.SingleAsync(x => x.Id == id);
			_dataContext.ToDoTasks.Remove(task);
			return (await _dataContext.SaveChangesAsync()) == 1;
		}

		public async Task<ToDoTask> GetTask(int id)
		{
			return await _dataContext.ToDoTasks.SingleOrDefaultAsync(x => x.Id == id);
		}

		public async Task<List<ToDoTask>> GetTasks()
		{
			return await _dataContext.ToDoTasks.ToListAsync();
		}

		public async Task<List<ToDoTask>> GetTasks(string userId)
		{
			return await _dataContext.ToDoTasks.Where(x=>x.UserId == userId).ToListAsync();
		}

		public async Task<bool> UpdateTask(ToDoTask task)
		{
			_dataContext.ToDoTasks.Update(task);
			return (await _dataContext.SaveChangesAsync() == 1);
		}
	}
}
