// File: Application/Interfaces/Repositories/IBookingRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Repository
{
    public interface IBookingRepository
    {
        // Get all booking and sort by start time
        Task<List<Booking>> GetAll();

        // Get booking via id
        Task<Booking?> GetById(int id);

        // Overlap checks
        Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc);
        Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc, int excludeBookingId);

        // Create
        Task<Booking> Add(Booking booking);

        // Update
        Task<Booking?> Update(int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title);

        // Delete
        Task<bool> Delete(int id);
    }
}


// The interface tells my repository what it has to offer