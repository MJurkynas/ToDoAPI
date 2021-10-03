using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Contracts;
using ToDoAPI.Data.Entities;
using ToDoAPI.Data.Repositories;

namespace ToDoAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class TaskController : ControllerBase
	{
		private readonly ITaskRepository _taskRepository;

		public TaskController(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		/// <summary>
		/// Get all user's tasks.
		/// </summary>
		/// <returns>A list of user's tasks.</returns>
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			IEnumerable<ToDoTask> tasks = new List<ToDoTask>();
			if (User.IsInRole("Administrator"))
			{
				tasks = await _taskRepository.GetTasks();
			}
			else
			{
				tasks = await _taskRepository.GetTasks(User.Claims.Single(x => x.Type == "id").Value);
			}

			return Ok(tasks.Select(x => new TaskResponse
			{
				Id = x.Id,
				UserId = x.UserId,
				IsCompleted = x.IsCompleted,
				Name = x.Name
			}));
		}

		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="task">Task to create.</param>
		/// <returns>The task that was created.</returns>
		[HttpPost]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Post(CreateTaskRequest task)
		{
			if (task == null)
			{
				return BadRequest("No data was provided");
			}

			if (task.Name != null)
			{
				ToDoTask toDoTask = new ToDoTask
				{
					UserId = User.Claims.Single(x=>x.Type == "id").Value,
					Name = task.Name,
					IsCompleted = task.IsCompleted
				};

				if (await _taskRepository.CreateTask(toDoTask))
				{
					return Ok(new TaskResponse
					{
						Id = toDoTask.Id,
						UserId = toDoTask.UserId,
						IsCompleted = toDoTask.IsCompleted,
						Name = toDoTask.Name
					});
				}
			}
			else
			{
				return BadRequest("Name is required");
			}
			return NoContent();
		}

		/// <summary>
		/// Update an existing task.
		/// </summary>
		/// <param name="id">Id of the task.</param>
		/// <param name="task">Task with its updated values.</param>
		/// <returns>The task that was updated.</returns>
		[HttpPut]
		[Route("{id}")]
		[Authorize(Roles = "User")]
		public async Task<IActionResult> Put([FromRoute]int id, [FromBody]CreateTaskRequest task)
		{
			if(task == null)
			{
				return BadRequest("No data was provided");
			}
			ToDoTask toDoTask = await _taskRepository.GetTask(id);
			if (toDoTask != null)
			{
				var userid = User.Claims.Single(x => x.Type == "id").Value;
				if (toDoTask.UserId == User.Claims.Single(x => x.Type == "id").Value)
				{
					if (task.Name != null)
					{
						toDoTask.Name = task.Name;
						toDoTask.IsCompleted = task.IsCompleted;

						if (await _taskRepository.UpdateTask(toDoTask))
						{
							return Ok(new TaskResponse
							{
								Id = toDoTask.Id,
								UserId = toDoTask.UserId,
								IsCompleted = toDoTask.IsCompleted,
								Name = toDoTask.Name
							});
						}
					}
					else
					{
						return BadRequest("Name is required");
					}
				}
				else
				{
					return Unauthorized();
				}
			}
			return NotFound();
		}

		/// <summary>
		/// Deletes the specified task.
		/// </summary>
		/// <param name="id">Id of the task to delete.</param>
		/// <returns>An empty 200 response.</returns>
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			ToDoTask toDoTask = await _taskRepository.GetTask(id);
			if (toDoTask != null)
			{
				if (User.IsInRole("Administrator") || toDoTask.UserId == User.Claims.Single(x => x.Type == "id").Value)
				{
					if (await _taskRepository.DeleteTask(id))
					{
						return Ok();
					}
				}
				else
				{
					return Unauthorized();
				}
			}
			return NotFound();
		}
	}
}
