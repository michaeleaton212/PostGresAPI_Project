using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Contracts;
using PostGresAPI.Extensions;

namespace PostGresAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookings;
        private readonly IRoomRepository _rooms;

        public BookingService(IBookingRepository bookings, IRoomRepository rooms) // Constructor Injection
        {
            _bookings = bookings;
            _rooms = rooms;
        }

        // Read
        public async Task<List<BookingDto>> GetAll()
        {
            var list = await _bookings.GetAll();
            return list.Select(b => b.ToDto()).ToList();
        }

        public async Task<BookingDto?> GetById(int id)
        {
            var b = await _bookings.GetById(id);
            return b is null ? null : b.ToDto();
        }

        public async Task<List<BookingDto>> GetByRoomId(int roomId)
        {
            var list = await _bookings.GetByRoomId(roomId);
            return list.Select(b => b.ToDto()).ToList();
        }

        public async Task<List<BookingDto>> GetByName(string name)
        {
            var list = await _bookings.GetByName(name);
            return list.Select(b => b.ToDto()).ToList();
        }

        // Helper
        public bool IsActive(BookingDto booking, DateTimeOffset atUtc)
            => booking.StartTime <= atUtc && atUtc < booking.EndTime;

        // Create
        public async Task<(bool Ok, string? Error, BookingDto? Result)> Create(CreateBookingDto createBookingDto)
        {
            if (createBookingDto.StartUtc >= createBookingDto.EndUtc)
                return (false, "Start must be before End.", null);

            if (!await _rooms.Exists(createBookingDto.RoomId))
                return (false, $"Room {createBookingDto.RoomId} not found.", null);

            if (await _bookings.HasOverlap(createBookingDto.RoomId, createBookingDto.StartUtc, createBookingDto.EndUtc))
                return (false, "Time range already booked.", null);

            var created = await _bookings.Add(createBookingDto);

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

            var updated = await _bookings.Update(id, updateBookingDto.StartUtc, updateBookingDto.EndUtc, updateBookingDto.Title);
            if (updated is null)
                return (false, "Booking not found.", null);

            return (true, null, updated.ToDto());
        }

        // Update Status
        public async Task<(bool Ok, string? Error, BookingDto? Result)> UpdateStatus(int id, UpdateBookingStatusDto updateStatusDto)
        {
            if (!Enum.TryParse<BookingStatus>(updateStatusDto.Status, true, out var status))
                return (false, "Invalid status. Valid values are: Pending, CheckedIn, CheckedOut, Cancelled", null);

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