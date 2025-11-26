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
        Task<(bool Ok, string? Error, BookingDto? Result)> Create(CreateBookingDto createBookingDto);

        // Update
        Task<(bool Ok, string? Error, BookingDto? Result)> Update(int id, UpdateBookingDto updateBookingDto);

        // Delete
        Task<(bool Ok, string? Error)> Delete(int id);

        Task<int?> GetRoomIdByCredentials(string bookingNumber, string name);

    }
}

// The interface tells my service what it has to offer