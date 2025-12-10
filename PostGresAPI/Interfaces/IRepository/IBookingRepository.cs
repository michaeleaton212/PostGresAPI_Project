// File: Application/Interfaces/Repositories/IBookingRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.Repository
{
    public interface IBookingRepository
    {
        // Get all booking and sort by start time
        Task<List<Booking>> GetAll();

        // Get booking via id
        Task<Booking?> GetById(int id);

        // Get booking via booking number
        Task<Booking?> GetByBookingNumber(string bookingNumber);

        // Get bookings by name
        Task<List<Booking>> GetByName(string name);

        // Get bookings by room ID
        Task<List<Booking>> GetByRoomId(int roomId);

        // Overlap checks
        Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc);
        Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc, int excludeBookingId);

        // Create
        Task<Booking> Add(CreateBookingDto createBookingDto);

        // Update
        Task<Booking?> Update(int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title);

        // Update Status
        Task<Booking?> UpdateStatus(int id, BookingStatus status);

        // Delete
        Task<bool> Delete(int id);
    }
}


// The interface tells my repository what it has to offer