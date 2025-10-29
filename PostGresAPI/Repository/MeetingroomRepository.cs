using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

using PostGresAPI.Extensions;


namespace PostGresAPI.Repository;

public class MeetingroomRepository : IMeetingroomRepository
{
    private readonly ApplicationDbContext _db;

    public MeetingroomRepository(ApplicationDbContext db) => _db = db;

    // Get all meetingrooms
    public async Task<List<Meetingroom>> GetAll()
    {
        try
        {
            return await _db.Meetingrooms
                .AsNoTracking()
                .OrderBy(m => m.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Abrufen aller Meetingrooms", ex);
        }
    }

    // Get by id
    public async Task<Meetingroom?> GetById(int id)
    {
        try
        {
            return await _db.Meetingrooms
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Abrufen von Meetingroom mit ID {id}", ex);
        }
    }

    // Create
    public async Task<Meetingroom> Add(CreateMeetingroomDto createMeetingroomDto) 
    {
        try
        {
            var meetingroom = createMeetingroomDto.ToEntity();
            _db.Meetingrooms.Add(meetingroom);
            await _db.SaveChangesAsync();
            return meetingroom;
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Hinzufügen des Meetingrooms", ex);
        }
    }

    // Update
    public async Task<Meetingroom?> Update(int id, string name, int numberOfChairs)
    {
        try
        {
            var entity = await GetById(id);
            if (entity is null)
                return null;

            entity.ApplyUpdate(name, numberOfChairs);
            _db.Meetingrooms.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Aktualisieren von Meetingroom mit ID {id}", ex);
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

            _db.Meetingrooms.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Löschen von Meetingroom mit ID {id}", ex);
        }
    }
}