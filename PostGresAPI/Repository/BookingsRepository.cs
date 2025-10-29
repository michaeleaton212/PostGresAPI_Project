using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;
using PostGresAPI.Contracts;
using PostGresAPI.Extensions;

namespace PostGresAPI.Repository;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _db;

    public BookingRepository(ApplicationDbContext db) => _db = db;

    // Get all booking and sort by start time
    public async Task<List<Booking>> GetAll()
    {
        try
        {
            return await _db.Bookings
                .AsNoTracking()
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Abrufen aller Bookings", ex);
        }
    }

    // Get booking via id
    public async Task<Booking?> GetById(int id)
    {
        try
        {
            return await _db.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Abrufen von Booking mit ID {id}", ex);
        }
    }

    // Check if there are overlapping bookings by Create
    public async Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc)
    {
        try
        {
            return await _db.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.StartTime < toUtc &&
                b.EndTime > fromUtc);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Pr�fen auf �berschneidungen f�r Room {roomId}", ex);
        }
    }

    // Check if overlapping by update
    public async Task<bool> HasOverlap(int roomId, DateTimeOffset fromUtc, DateTimeOffset toUtc, int excludeBookingId)
    {
        try
        {
            return await _db.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.Id != excludeBookingId &&
                b.StartTime < toUtc &&
                b.EndTime > fromUtc);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Pr�fen auf �berschneidungen f�r Room {roomId} (au�er Booking {excludeBookingId})", ex);
        }
    }

    // Create
    public async Task<Booking> Add(CreateBookingDto createBookingDto)
    {
        try
        {
            var booking = createBookingDto.ToEntity();
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
            return booking;
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Hinzuf�gen des Bookings", ex);
        }
    }

    // Update
    public async Task<Booking?> Update(int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title)
    {
        try
        {
            var entity = await GetById(id);
            if (entity is null)
                return null;

            entity.ApplyUpdate(startUtc, endUtc, title);
            _db.Bookings.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Aktualisieren von Booking mit ID {id}", ex);
        }
    }

    // Delete
    public async Task<bool> Delete(int id)
    {
        try
        {
            var entity = await GetById(id);
            if (entity is null)
                return false;

            _db.Bookings.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim L�schen von Booking mit ID {id}", ex);
        }
    }
}