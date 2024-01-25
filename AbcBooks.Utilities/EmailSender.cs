using AbcBooks.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AbcBooks.Utilities
{
	public class EmailSender : IEmailSender
	{
		private readonly SmtpConfig _smtpConfig;

		public EmailSender(IOptions<SmtpConfig> smtpConfig)
        {
			_smtpConfig = smtpConfig.Value;
		}

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var emailToSend = new MimeMessage();
			emailToSend.From.Add(MailboxAddress.Parse(_smtpConfig.SenderAddress));
			emailToSend.To.Add(MailboxAddress.Parse(email));
			emailToSend.Subject = subject;
			emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

			using (var emailClient = new SmtpClient())
			{
				emailClient.Connect(_smtpConfig.Host, _smtpConfig.Port);
				if (_smtpConfig.UserName is not null && _smtpConfig.UserName is not "")
				{
					emailClient.Authenticate(_smtpConfig.UserName, _smtpConfig.Password); 
				}
				emailClient.Send(emailToSend);
				emailClient.Disconnect(true);
			}

			return Task.CompletedTask;
		}
	}
}
