// Services/GmailEmailSender.cs
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;
using _LAB__QC_HQ.Models;
using Microsoft.Extensions.Logging;

namespace _LAB__QC_HQ.Services
{
    public class GmailEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<GmailEmailSender> _logger;

        public GmailEmailSender(IOptions<EmailSettings> emailSettings, ILogger<GmailEmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // LOG 1: Method called
            Console.WriteLine($"📧 [1] SendEmailAsync CALLED at {DateTime.Now:HH:mm:ss}");
            Console.WriteLine($"   To: {email}");
            Console.WriteLine($"   Subject: {subject}");

            // Also log to file
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] SendEmailAsync called for: {email}, Subject: {subject}\n";
            File.AppendAllText("email-debug.log", logMessage);

            try
            {
                // LOG 2: Creating message
                Console.WriteLine($"📧 [2] Creating email message...");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };

                // LOG 3: Extracting reset link (for debugging)
                var linkStart = htmlMessage.IndexOf("href=\"");
                if (linkStart > -1)
                {
                    linkStart += 6;
                    var linkEnd = htmlMessage.IndexOf("\"", linkStart);
                    if (linkEnd > linkStart)
                    {
                        var link = htmlMessage.Substring(linkStart, linkEnd - linkStart);
                        link = System.Net.WebUtility.HtmlDecode(link);
                        Console.WriteLine($"📧 [3] Reset link in email: {link}");
                        File.AppendAllText("email-debug.log", $"   Reset link: {link}\n");
                    }
                }

                // LOG 4: Connecting to SMTP
                Console.WriteLine($"📧 [4] Connecting to {_emailSettings.SmtpServer}:{_emailSettings.SmtpPort}...");

                using var client = new SmtpClient();

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, false);
                Console.WriteLine($"📧 [5] Connected. Authenticating...");

                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
                Console.WriteLine($"📧 [6] Authenticated. Sending...");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                // LOG 7: Success
                Console.WriteLine($"✅ [7] Email sent successfully to {email}");
                File.AppendAllText("email-debug.log", $"✅ SUCCESS: Email sent\n");

                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                // LOG 8: Error
                Console.WriteLine($"❌ [8] FAILED to send email to {email}: {ex.Message}");
                Console.WriteLine($"   Exception type: {ex.GetType().Name}");

                File.AppendAllText("email-debug.log", $"❌ FAILED: {ex.Message}\n");
                _logger.LogError(ex, $"Failed to send email to {email}");

                // Re-throw so Identity knows email failed
                throw;
            }
        }
    }
}