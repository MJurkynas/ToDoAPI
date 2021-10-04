using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Models;

namespace ToDoAPI.Services
{
	public interface IMailService
	{
		Task SendEmail(Email email);
	}
}
