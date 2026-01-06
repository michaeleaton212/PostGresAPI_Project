using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Contracts;
using PostGresAPI.Extensions;
using PostGresAPI.Data;
using PostGresAPI.Persistence.Entities;
using Microsoft.Extensions.Logging;

namespace PostGresAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookings;
        private readonly IRoomRepository _rooms;
        private readonly ApplicationDbContext _db;
        private readonly ISmtpEmailService _emailService;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookings,
            IRoomRepository rooms,
            ApplicationDbContext db,
            ISmtpEmailService emailService,
            ILogger<BookingService> logger) // Constructor Injection
        {
            _bookings = bookings;
            _rooms = rooms;
            _db = db;
            _emailService = emailService;
            _logger = logger;
        }

        // Read
        public async Task<List<BookingDto>> GetAll()
        {
            await UpdateExpiredBookings();
            var list = await _bookings.GetAll();
            return list.Select(b => b.ToDto()).ToList();
        }

        public async Task<BookingDto?> GetById(int id)
        {
            await UpdateExpiredBookings();
            var b = await _bookings.GetById(id);
            return b is null ? null : b.ToDto();
        }

        public async Task<List<BookingDto>> GetByRoomId(int roomId)
        {
            await UpdateExpiredBookings();
            var list = await _bookings.GetByRoomId(roomId);
            return list.Select(b => b.ToDto()).ToList();
        }

        public async Task<List<BookingDto>> GetByName(string name)
        {
            await UpdateExpiredBookings();
            var list = await _bookings.GetByName(name);
            return list.Select(b => b.ToDto()).ToList();
        }

        public async Task<List<BookingDto>> GetByIds(List<int> ids)
        {
            await UpdateExpiredBookings();
            var list = await _bookings.GetByIds(ids);
            return list.Select(b => b.ToDto()).ToList();
        }

        public async Task<List<BookingDto>> GetByUserId(int userId)
        {
            await UpdateExpiredBookings();
            var list = await _bookings.GetByUserId(userId);
            return list.Select(b => b.ToDto()).ToList();
        }

        // Helper method to automatically update expired bookings
        private async Task UpdateExpiredBookings()
        {
            var allBookings = await _bookings.GetAll();
            var now = DateTimeOffset.UtcNow;

            foreach (var booking in allBookings)
            {
                // Only update bookings that are Pending or CheckedIn and have ended
                if ((booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.CheckedIn)
                    && booking.EndTime < now)
                {
                    await _bookings.UpdateStatus(booking.Id, BookingStatus.Expired);
                }
            }
        }

        // Helper
        public bool IsActive(BookingDto booking, DateTimeOffset atUtc)
            => booking.StartTime <= atUtc && atUtc < booking.EndTime;

        // Create
        public async Task<(bool Ok, string? Error, BookingDto? Result)> Create(CreateBookingDto createBookingDto)
        {
            _logger.LogInformation("=== BOOKING CREATION START ===");
            _logger.LogInformation("CreateBookingDto - RoomId: {RoomId}, UserId: {UserId}, StartUtc: {Start}, EndUtc: {End}", 
                createBookingDto.RoomId, 
                createBookingDto.UserId?.ToString() ?? "NULL", 
                createBookingDto.StartUtc, 
                createBookingDto.EndUtc);

            if (createBookingDto.StartUtc >= createBookingDto.EndUtc)
                return (false, "Start must be before End.", null);

            if (!await _rooms.Exists(createBookingDto.RoomId))
                return (false, $"Room {createBookingDto.RoomId} not found.", null);

            if (await _bookings.HasOverlap(createBookingDto.RoomId, createBookingDto.StartUtc, createBookingDto.EndUtc))
                return (false, "Time range already booked.", null);

            // 1) Booking erstellen
            var created = await _bookings.Add(createBookingDto);
            _logger.LogInformation("Booking created with Id: {Id}, BookingNumber: {BookingNumber}, UserId: {UserId}", 
                created.Id, created.BookingNumber, created.UserId?.ToString() ?? "NULL");

            // 2) Room-Nav laden (falls Repository es nicht mitliefert)
            if (created.Room is null)
            {
                _logger.LogInformation("Room not loaded, fetching Room #{RoomId}...", created.RoomId);
                var room = await _rooms.GetById(created.RoomId);
                if (room is not null)
                {
                    created.Room = room;
                    _logger.LogInformation("Room loaded: {RoomName}", room.Name);
                }
                else
                {
                    _logger.LogWarning("Room #{RoomId} not found in database", created.RoomId);
                }
            }

            // 3) Empfänger E-Mail bestimmen (über UserId, da Booking FK optional hat)
            string? recipientEmail = null;

            if (created.UserId.HasValue)
            {
                _logger.LogInformation("Fetching user #{UserId} to get email address...", created.UserId.Value);
                var user = await _db.Users.FindAsync(created.UserId.Value);
                if (user != null)
                {
                    recipientEmail = user.Email;
                    _logger.LogInformation("User found: {UserName}, Email: {Email}", user.UserName, recipientEmail ?? "NULL");
                }
                else
                {
                    _logger.LogWarning("? User #{UserId} not found in database", created.UserId.Value);
                }
            }
            else
            {
                _logger.LogWarning("? No UserId in booking, cannot send email");
            }

            // 4) Email: Buchungsbestätigung senden
            if (!string.IsNullOrWhiteSpace(recipientEmail))
            {
                _logger.LogInformation("Calling SMTP email service to send confirmation email...");
                
                var subject = $"Buchungsbestätigung - {created.BookingNumber}";
                var htmlBody = EmailTemplates.BookingConfirmationHtml(created);
                var textBody = EmailTemplates.BookingConfirmationText(created);
                
                var emailSent = await _emailService.SendEmailAsync(recipientEmail, subject, htmlBody, textBody);
                _logger.LogInformation("Email sending result: {Result}", emailSent ? "SUCCESS" : "FAILED");
            }
            else
            {
                _logger.LogWarning("? No recipient email address, skipping email sending");
            }

            _logger.LogInformation("=== BOOKING CREATION END ===");
            return (true, null, created.ToDto());
        }

        // Update
        public async Task<(bool Ok, string? Error, BookingDto? Result)> Update(int id, UpdateBookingDto updateBookingDto)
        {
            if (updateBookingDto.StartUtc >= updateBookingDto.EndUtc)
                return (false, "Start must be before End.", null);

            var existing = await _bookings.GetById(id);
            if (existing is null)
                return (false, "Booking not found.", null);

            var hasOverlap = await _bookings.HasOverlap(existing.RoomId, updateBookingDto.StartUtc, updateBookingDto.EndUtc, excludeBookingId: id);
            if (hasOverlap)
                return (false, "Time range already booked.", null);

            var updated = await _bookings.Update(id, updateBookingDto.StartUtc, updateBookingDto.EndUtc, updateBookingDto.Title, updateBookingDto.NumberOfPersons);
            if (updated is null)
                return (false, "Booking not found.", null);

            return (true, null, updated.ToDto());
        }

        // Update Status
        public async Task<(bool Ok, string? Error, BookingDto? Result)> UpdateStatus(int id, UpdateBookingStatusDto updateStatusDto)
        {
            if (!Enum.TryParse<BookingStatus>(updateStatusDto.Status, true, out var status))
                return (false, "Invalid status. Valid values are: Pending, CheckedIn, Expired, Cancelled", null);

            var existing = await _bookings.GetById(id);
            if (existing is null)
                return (false, "Booking not found.", null);

            var updated = await _bookings.UpdateStatus(id, status);
            if (updated is null)
                return (false, "Booking not found.", null);

            return (true, null, updated.ToDto());
        }

        // Delete
        public async Task<(bool Ok, string? Error)> Delete(int id)
        {
            var ok = await _bookings.Delete(id);
            return ok ? (true, null) : (false, "Booking not found.");
        }

        // Login (Booking Ids) - Unterstützt mehrere Buchungen auf denselben Namen
        public async Task<List<int>> GetBookingIdsByCredentials(string bookingNumber, string name)
        {
            if (string.IsNullOrWhiteSpace(bookingNumber) || string.IsNullOrWhiteSpace(name))
                return new List<int>();

            var b = await _bookings.GetByBookingNumber(bookingNumber.Trim());
            if (b is null) return new List<int>();

            var matches = string.Equals(
                (b.Title ?? string.Empty).Trim(),
                name.Trim(),
                StringComparison.OrdinalIgnoreCase);

            if (!matches) return new List<int>();

            // Alle Buchungen mit demselben Namen finden
            var allBookings = await _bookings.GetByName(name.Trim());
            return allBookings.Select(booking => booking.Id).ToList();
        }
    }
}
