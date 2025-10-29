using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;

namespace PostGresAPI.Repository;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _db;

    public RoomRepository(ApplicationDbContext db) => _db = db;

    // Get all
    public async Task<List<Room>> GetAll()
    {
        try
        {
            return await _db.Rooms
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Abrufen aller Rooms", ex);
        }
    }

    // Get Room by id
    public async Task<Room?> GetById(int id)
    {
        try
        {
            return await _db.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Abrufen von Room mit ID {id}", ex);
        }
    }

    // Get Meetingrooms
    public async Task<List<Meetingroom>> GetMeetingrooms()
    {
        try
        {
            return await _db.Rooms
                .AsNoTracking()
                .OfType<Meetingroom>()
                .OrderBy(r => r.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Abrufen aller Meetingrooms", ex);
        }
    }

    // Get Bedrooms
    public async Task<List<Bedroom>> GetBedrooms()
    {
        try
        {
            return await _db.Rooms
                .AsNoTracking()
                .OfType<Bedroom>()
                .OrderBy(r => r.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Abrufen aller Bedrooms", ex);
        }
    }

    // Creates a new Room
    public async Task<Room> Add(Room room)
    {
        try
        {
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
            return room;
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Hinzuf�gen des Rooms", ex);
        }
    }

    // Updates an already existing Room
    public async Task<Room?> UpdateName(int id, string name)
    {
        try
        {
            var entity = await GetById(id);
            if (entity is null)
                return null;

            entity.SetName(name);
            _db.Rooms.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Aktualisieren von Room mit ID {id}", ex);
        }
    }

    // Delete Room by Id
    public async Task<bool> Delete(int id)
    {
        try
        {
            var entity = await GetById(id);
            if (entity is null)
                return false;

            _db.Rooms.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim L�schen von Room mit ID {id}", ex);
        }
    }

    // Check if Room exists
    public async Task<bool> Exists(int id)
    {
        try
        {
            return await _db.Rooms.AnyAsync(r => r.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Pr�fen der Existenz von Room mit ID {id}", ex);
        }
    }
}