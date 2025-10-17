using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Contracts;

namespace PostGresAPI.Services
{
    public interface IBookingService
    {
        // Read
        Task<List<BookingDto>> GetAll();
        Task<BookingDto?> GetById(int id);

        // Check if booking is active at given time
        bool IsActive(BookingDto booking, DateTimeOffset atUtc);

        // Create
        Task<(bool Ok, string? Error, BookingDto? Result)> Create(
            int roomId, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title);

        // Update
        Task<(bool Ok, string? Error, BookingDto? Result)> Update(
            int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title);

        // Delete
        Task<(bool Ok, string? Error)> Delete(int id);
    }
}

// The interface tells my service what it has to offer