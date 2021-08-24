using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Phrook.Models.Options;
using Polly;

namespace Phrook.Models.Services.Infrastructure
{
	public class MailKitEmailSender : IEmailSender
	{
		private readonly IOptionsMonitor<SmtpOptions> smtpOptionsMonitor;
        private readonly ILogger<MailKitEmailSender> logger;
        public MailKitEmailSender(IOptionsMonitor<SmtpOptions> smtpOptionsMonitor, ILogger<MailKitEmailSender> logger)
        {
            this.logger = logger;
            this.smtpOptionsMonitor = smtpOptionsMonitor;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var options = this.smtpOptionsMonitor.CurrentValue;

                using  var client = new SmtpClient();
                await client.ConnectAsync(options.Host, options.Port, options.Security);
                if (!string.IsNullOrEmpty(options.Username))
                {
                    await client.AuthenticateAsync(options.Username, options.Password);
                }

                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(options.Sender));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };

				//using Polly to try again if sending fails 
				//it tries 3 times, the first one is by default, the others -2- are specified in WaitAndRetryAsync()
				var policy = Policy
					.Handle<Exception>()
					.WaitAndRetryAsync(2, retry => TimeSpan.FromMilliseconds(200));
				await policy.ExecuteAsync(() =>  client.SendAsync(message));
                await client.DisconnectAsync(true);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Couldn't send email to {email} with message {message}", email, htmlMessage);
            }
        }
	}
}