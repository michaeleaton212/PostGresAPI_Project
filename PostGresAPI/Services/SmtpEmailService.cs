using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PostGresAPI.Services
{
    public interface ISmtpEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? textBody = null);
    }

    public class SmtpEmailService : ISmtpEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var host = _configuration["Smtp:Host"];
            var port = _configuration["Smtp:Port"];
            var username = _configuration["Smtp:Username"];
            
            _logger.LogInformation("SmtpEmailService initialized with Host: {Host}, Port: {Port}, Username: {Username}",
                string.IsNullOrEmpty(host) ? "MISSING" : host,
                string.IsNullOrEmpty(port) ? "MISSING" : port,
                string.IsNullOrEmpty(username) ? "MISSING" : username);
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? textBody = null)
        {
            _logger.LogInformation("=== EMAIL SENDING START ===");
            _logger.LogInformation("Attempting to send email to: {Email}, Subject: {Subject}", toEmail, subject);

            try
            {
                var host = _configuration["Smtp:Host"];
                var port = int.Parse(_configuration["Smtp:Port"] ?? "587");
                var useSsl = bool.Parse(_configuration["Smtp:UseSsl"] ?? "true");
                var username = _configuration["Smtp:Username"];
                var password = _configuration["Smtp:Password"];
                var fromEmail = _configuration["Smtp:FromEmail"] ?? username;
                var fromName = _configuration["Smtp:FromName"] ?? "Hotel Management";

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogError("? SMTP configuration is incomplete. Please check appsettings.json");
                    return false;
                }

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = useSsl,
                    Credentials = new NetworkCredential(username, password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail ?? username ?? "", fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                if (!string.IsNullOrEmpty(textBody))
                {
                    mailMessage.AlternateViews.Add(
                        AlternateView.CreateAlternateViewFromString(textBody, null, "text/plain")
                    );
                }

                _logger.LogInformation("Sending email via SMTP: {Host}:{Port}", host, port);
                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("? Email sent successfully!");
                _logger.LogInformation("=== EMAIL SENDING SUCCESS ===");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Failed to send email: {Message}", ex.Message);
                _logger.LogInformation("=== EMAIL SENDING FAILED ===");
                return false;
            }
        }
    }
}
