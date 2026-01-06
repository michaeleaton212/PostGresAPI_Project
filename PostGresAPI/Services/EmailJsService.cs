using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostGresAPI.Models;

namespace PostGresAPI.Services
{
    public interface IEmailJsService
    {
        Task<bool> SendBookingConfirmationAsync(Booking booking, string recipientEmail);
    }

    public class EmailJsService : IEmailJsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceId;
        private readonly string _templateId;
        private readonly string _publicKey;
        private readonly ILogger<EmailJsService> _logger;

        public EmailJsService(HttpClient httpClient, IConfiguration configuration, ILogger<EmailJsService> logger)
        {
            _httpClient = httpClient;
            _serviceId = configuration["EmailJs:ServiceId"] ?? "";
            _templateId = configuration["EmailJs:TemplateId"] ?? "";
            _publicKey = configuration["EmailJs:PublicKey"] ?? "";
            _logger = logger;

            _logger.LogInformation("EmailJsService initialized with ServiceId: {ServiceId}, TemplateId: {TemplateId}, PublicKey: {PublicKey}",
                string.IsNullOrEmpty(_serviceId) ? "MISSING" : _serviceId,
                string.IsNullOrEmpty(_templateId) ? "MISSING" : _templateId,
                string.IsNullOrEmpty(_publicKey) ? "MISSING" : _publicKey.Substring(0, Math.Min(4, _publicKey.Length)) + "...");
        }

        public async Task<bool> SendBookingConfirmationAsync(Booking booking, string recipientEmail)
        {
            _logger.LogInformation("=== EMAIL SENDING START ===");
            _logger.LogInformation("Attempting to send booking confirmation email to: {Email} for Booking #{BookingNumber}", 
                recipientEmail, booking.BookingNumber);

            if (string.IsNullOrWhiteSpace(_serviceId) || 
                string.IsNullOrWhiteSpace(_templateId) || 
                string.IsNullOrWhiteSpace(_publicKey))
            {
                _logger.LogError("EmailJS configuration is incomplete. ServiceId: {ServiceId}, TemplateId: {TemplateId}, PublicKey: {PublicKey}",
                    string.IsNullOrEmpty(_serviceId) ? "MISSING" : "OK",
                    string.IsNullOrEmpty(_templateId) ? "MISSING" : "OK",
                    string.IsNullOrEmpty(_publicKey) ? "MISSING" : "OK");
                return false;
            }

            var duration = booking.EndTime - booking.StartTime;
            var durationText = GetDurationText(duration);

            var roomType = GetRoomType(booking.Room);
            var totalPrice = CalculateTotalPrice(booking, duration);

            var templateParams = new
            {
                to_email = recipientEmail,
                booking_number = booking.BookingNumber,
                room_name = booking.Room?.Name ?? "N/A",
                room_type = roomType,
                check_in = booking.StartTime.ToString("dd.MM.yyyy HH:mm"),
                check_out = booking.EndTime.ToString("dd.MM.yyyy HH:mm"),
                number_of_persons = booking.NumberOfPersons,
                duration = durationText,
                total_price = $"{totalPrice:F2} CHF",
                guest_name = booking.Title ?? "Gast"
            };

            var payload = new
            {
                service_id = _serviceId,
                template_id = _templateId,
                user_id = _publicKey,
                template_params = templateParams
            };

            try
            {
                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
                _logger.LogInformation("Email payload prepared: {Payload}", json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending POST request to EmailJS API...");
                var response = await _httpClient.PostAsync(
                    "https://api.emailjs.com/api/v1.0/email/send",
                    content
                );

                var responseBody = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("? Email sent successfully! Response: {Response}", responseBody);
                    _logger.LogInformation("=== EMAIL SENDING SUCCESS ===");
                    return true;
                }
                else
                {
                    _logger.LogError("? Email sending failed. Status: {StatusCode}, Response: {Response}", 
                        response.StatusCode, responseBody);
                    _logger.LogInformation("=== EMAIL SENDING FAILED ===");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Exception occurred while sending email: {Message}", ex.Message);
                _logger.LogInformation("=== EMAIL SENDING EXCEPTION ===");
                return false;
            }
        }

        private string GetRoomType(Room? room)
        {
            if (room == null) return "Unbekannt";
            
            return room switch
            {
                Bedroom => "Schlafzimmer",
                Meetingroom => "Meetingraum",
                _ => "Raum"
            };
        }

        private decimal CalculateTotalPrice(Booking booking, TimeSpan duration)
        {
            var room = booking.Room;
            if (room == null) return 0;

            decimal pricePerNight = room switch
            {
                Bedroom bedroom => bedroom.PricePerNight,
                Meetingroom => 100m,
                _ => 0
            };

            var nights = (int)Math.Ceiling(duration.TotalDays);
            return pricePerNight * nights * booking.NumberOfPersons;
        }

        private string GetDurationText(TimeSpan duration)
        {
            if (duration.TotalDays >= 1)
            {
                var days = (int)Math.Ceiling(duration.TotalDays);
                return days == 1 ? "1 Nacht" : $"{days} Nächte";
            }
            else if (duration.TotalHours >= 1)
            {
                var hours = (int)Math.Ceiling(duration.TotalHours);
                return hours == 1 ? "1 Stunde" : $"{hours} Stunden";
            }
            else
            {
                var minutes = (int)Math.Ceiling(duration.TotalMinutes);
                return $"{minutes} Minuten";
            }
        }
    }
}
