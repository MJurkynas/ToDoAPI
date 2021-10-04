using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Models
{
	public class Email
	{
		public MailboxAddress To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
