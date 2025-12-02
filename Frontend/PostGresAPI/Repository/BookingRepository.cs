using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;
using PostGresAPI.Contracts;
using PostGresAPI.Extensions;

namespace PostGresAPI.Repository
{
    public class BookingRepository : IBookingRepository
  {
        private readonly ApplicationDbContext _context;

  public BookingRepository(ApplicationDbContext context)
  {
            _context = context;
        }

        // Get all bookings sorted by start time
        public async Task<List<Booking>> GetAll()
  {
            return await _context.Bookings
            .OrderBy(b => b.StartTime)
        .ToListAsync();
}

        // Get booking by id
  public async Task<Booking?> GetById(int id)
        {
     return await _context.Bookings.FindAsync(id);
        }

        // Get booking by booking number
        public async Task<Booking?> GetByBookingNumber(string bookingNumber)
        {
            return await _context.Bookings
 .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
        }

        // Check for overlapping bookings
        public async Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc)
        {
       return await _context.Bookings
            .AnyAsync(b =>
          b.RoomId == roomId &&
   b.StartTime < toUtc &&
                  b.EndTime > fromUtc
                );
  }

        // Check for overlapping bookings (excluding a specific booking ID)
        public async Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc, int excludeBookingId)
        {
            return await _context.Bookings
        .AnyAsync(b =>
           b.Id != excludeBookingId &&
     b.RoomId == roomId &&
            b.StartTime < toUtc &&
         b.EndTime > fromUtc
     );
        }

        // Create a new booking
        public async Task<Booking> Add(CreateBookingDto createBookingDto)
 {
   // Convert DTO to Entity using extension method
            var booking = createBookingDto.ToEntity();

            // Add to database
        await _context.Bookings.AddAsync(booking);
  await _context.SaveChangesAsync();

          return booking;
        }

 // Update an existing booking
        public async Task<Booking?> Update(int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title)
     {
    var booking = await _context.Bookings.FindAsync(id);
      if (booking is null)
            return null;

       booking.StartTime = startUtc;
   booking.EndTime = endUtc;
      booking.Title = title;

await _context.SaveChangesAsync();

            return booking;
        }

        // Delete a booking
        public async Task<bool> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
       if (booking is null)
     return false;

        _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

    return true;
        }
    }
}
