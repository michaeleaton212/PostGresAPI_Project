using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Contracts;

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
            return list.Select(b => new BookingDto(b.Id, b.RoomId, b.StartTime, b.EndTime, b.Title)).ToList();
        }

        public async Task<BookingDto?> GetById(int id)
        {
            var b = await _bookings.GetById(id);
            return b is null ? null : new BookingDto(b.Id, b.RoomId, b.StartTime, b.EndTime, b.Title);
        }

        // Helper
        public bool IsActive(BookingDto booking, DateTimeOffset atUtc)
            => booking.StartTime <= atUtc && atUtc < booking.EndTime;

        // Create
        public async Task<(bool Ok, string? Error, BookingDto? Result)> Create(
            int roomId, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title)
        {
            if (startUtc >= endUtc)
                return (false, "Start must be before End.", null);

            if (!await _rooms.Exists(roomId))
                return (false, $"Room {roomId} not found.", null);

            if (await _bookings.HasOverlap(roomId, startUtc, endUtc))
                return (false, "Time range already booked.", null);

            var dto = new CreateBookingDto(roomId, startUtc, endUtc, title);
            var created = await _bookings.Add(dto);

            return (true, null, new BookingDto(created.Id, created.RoomId, created.StartTime, created.EndTime, created.Title));
        }


        // Update
        public async Task<(bool Ok, string? Error, BookingDto? Result)> Update(
            int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title)
        {
            if (startUtc >= endUtc)
                return (false, "Start must be before End.", null);

            var existing = await _bookings.GetById(id);
            if (existing is null)
                return (false, "Booking not found.", null);

            var hasOverlap = await _bookings.HasOverlap(existing.RoomId, startUtc, endUtc, excludeBookingId: id);
            if (hasOverlap)
                return (false, "Time range already booked.", null);

            var updated = await _bookings.Update(id, startUtc, endUtc, title);
            if (updated is null)
                return (false, "Booking not found.", null);

            return (true, null, new BookingDto(updated.Id, updated.RoomId, updated.StartTime, updated.EndTime, updated.Title));
        }

        // Delete
        public async Task<(bool Ok, string? Error)> Delete(int id)
        {
            var ok = await _bookings.Delete(id);
            return ok ? (true, null) : (false, "Booking not found.");
        }
    }
}