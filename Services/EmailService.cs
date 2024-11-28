using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CarRentalSystem.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string message)
        {
            var apiKey = Environment.GetEnvironmentVariable("REDACTED");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("SendGrid API key not found in environment variables.");
            }
            var client = new SendGridClient(apiKey);

            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];
            var from = new EmailAddress(fromEmail, fromName);

            var to = new EmailAddress(toEmail, toName);

            var emailMessage = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(emailMessage);

            if ((int)response.StatusCode >= 400)
            {
                throw new Exception($"Email sending failed: {response.StatusCode}");
            }
        }
    }
}
