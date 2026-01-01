// Services/ConsoleEmailSender.cs
using Microsoft.AspNetCore.Identity.UI.Services;

namespace _LAB__QC_HQ.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine("=== IDENTITY EMAIL ===");
            Console.WriteLine($"To: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Content Preview: {htmlMessage.Substring(0, Math.Min(100, htmlMessage.Length))}...");

            // Extract and show the reset link
            var linkStart = htmlMessage.IndexOf("href=\"");
            if (linkStart > -1)
            {
                linkStart += 6;
                var linkEnd = htmlMessage.IndexOf("\"", linkStart);
                if (linkEnd > linkStart)
                {
                    var link = htmlMessage.Substring(linkStart, linkEnd - linkStart);
                    Console.WriteLine($"Reset Link: {System.Net.WebUtility.HtmlDecode(link)}");
                }
            }

            return Task.CompletedTask;
        }
    }
}