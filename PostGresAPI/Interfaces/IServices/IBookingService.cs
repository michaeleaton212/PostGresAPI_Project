// File: Application/Interfaces/Services/IBookingService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Services
{
    public interface IBookingService
    {
        // Read
        Task<List<Booking>> GetAll();
        Task<Booking?> GetById(int id);

        // Helper
        bool IsActive(Booking booking, DateTimeOffset atUtc);

        // Create
        Task<(bool Ok, string? Error, Booking? Result)> Create(
            int roomId, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title);

        // Update
        Task<(bool Ok, string? Error, Booking? Result)> Update(
            int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title);

        // Delete
        Task<(bool Ok, string? Error)> Delete(int id);
    }
}
