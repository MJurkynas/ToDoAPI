using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using ToDoAPI.Models;
using ToDoAPI.Settings;

namespace ToDoAPI.Services
{
	public class MailService : IMailService
	{
		private readonly MailSettings _mailSettings;
		public MailService(MailSettings mailSettings)
		{
			_mailSettings = mailSettings;
		}
		public async Task SendEmail(Email email)
		{
			BodyBuilder builder = new BodyBuilder
			{
				HtmlBody = email.Body
			};
			MimeMessage message = new MimeMessage
			{
				Sender = MailboxAddress.Parse(_mailSettings.Mail),
				Subject = email.Subject,
				Body = builder.ToMessageBody()
			};
			message.To.Add(email.To);

			using (SmtpClient client = new SmtpClient())
			{
				client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
				client.Authenticate(_mailSettings.Mail, _mailSettings.Password);
				await client.SendAsync(message);
				client.Disconnect(true);
			}
		}
	}
}
