using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;

namespace PostGresAPI.Repository;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _db;

    public BookingRepository(ApplicationDbContext db) => _db = db;

    // Get all booking and sort by start time
    public Task<List<Booking>> GetAll() =>
        _db.Bookings
           .AsNoTracking()
           .OrderBy(b => b.StartTime)
           .ToListAsync();

    // Get booking via id
    public Task<Booking?> GetById(int id) =>
        _db.Bookings
           .AsNoTracking()
           .FirstOrDefaultAsync(b => b.Id == id);



    // Check if there are overlapping bookings by Create
    public Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc) =>
        _db.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.StartTime < toUtc &&
            b.EndTime > fromUtc);

    // Check if overlapping by update
    public Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc, int excludeBookingId) =>
        _db.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.Id != excludeBookingId &&
            b.StartTime < toUtc &&
            b.EndTime > fromUtc);

    // Create
    public async Task<Booking> Add(Booking booking)
    {
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();
        return booking;
    }

    // Update
    public async Task<Booking?> Update(int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title)
    {
        var entity = await _db.Bookings.FindAsync(id);
        if (entity is null)
            return null;

        entity.StartTime = startUtc;
        entity.EndTime = endUtc;
        entity.Title = title;

        _db.Bookings.Update(entity);
        await _db.SaveChangesAsync();

        return entity;
    }

    // Delete
    public async Task<bool> Delete(int id)
    {
        var entity = await _db.Bookings.FindAsync(id);
        if (entity is null)
            return false;

        _db.Bookings.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
