using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models; 

namespace PostGresAPI.Repository;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _db;

    public RoomRepository(ApplicationDbContext db) => _db = db;

    // Get all
    public Task<List<Room>> GetAll() =>
        _db.Rooms
           .AsNoTracking()
           .OrderBy(r => r.Id)
           .ToListAsync();

    // Get Room by id
    public Task<Room?> GetById(int id) =>
        _db.Rooms
           .AsNoTracking()
           .FirstOrDefaultAsync(r => r.Id == id);

    // Get Meetingrooms
    public Task<List<Meetingroom>> GetMeetingrooms() =>
        _db.Rooms
           .AsNoTracking()
           .OfType<Meetingroom>()
           .OrderBy(r => r.Id)
           .ToListAsync();

    // Get Bedrooms
    public Task<List<Bedroom>> GetBedrooms() =>
        _db.Rooms
           .AsNoTracking()
           .OfType<Bedroom>()
           .OrderBy(r => r.Id)
           .ToListAsync();

    // Creates a new Room
    public async Task<Room> Add(Room room)
    {
        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();
        return room;
    }

    // Updates an already existing Room
    public async Task<Room> Update(Room room)
    {
        _db.Rooms.Update(room);
        await _db.SaveChangesAsync();
        return room;
    }

    // Delete Room by Id
    public async Task<bool> Delete(int id)
    {
        var entity = await _db.Rooms.FindAsync(id);
        if (entity is null) return false;

        _db.Rooms.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    // Check if Room exists
    public Task<bool> Exists(int id) =>
        _db.Rooms.AnyAsync(r => r.Id == id);
}
