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
                    _logger.LogError("SMTP configuration is incomplete. Please check appsettings.json");
                    return false;
                }

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = useSsl,
                    Credentials = new NetworkCredential(username, password)
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail ?? username ?? "", fromName),
                    Subject = subject,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                var plainText = textBody ?? StripHtml(htmlBody);
                var plainView = AlternateView.CreateAlternateViewFromString(plainText, null, "text/plain");
                plainView.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
                mailMessage.AlternateViews.Add(plainView);

                var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
                htmlView.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
                mailMessage.AlternateViews.Add(htmlView);

                _logger.LogInformation("Sending email via SMTP: {Host}:{Port}", host, port);
                _logger.LogInformation("Email format: multipart/alternative (Plain + HTML)");
                
                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully!");
                _logger.LogInformation("=== EMAIL SENDING SUCCESS ===");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email: {Message}", ex.Message);
                _logger.LogInformation("=== EMAIL SENDING FAILED ===");
                return false;
            }
        }

  
        private static string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            var text = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
            text = System.Web.HttpUtility.HtmlDecode(text);
            return text.Trim();
        }
    }
}
