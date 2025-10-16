using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;  

namespace PostGresAPI.Repository;

public class MeetingroomRepository : IMeetingroomRepository
{
    private readonly ApplicationDbContext _db;

    public MeetingroomRepository(ApplicationDbContext db) => _db = db;

    // Get all meetingrooms
    public Task<List<Meetingroom>> GetAll() =>
        _db.Meetingrooms
           .AsNoTracking()
           .OrderBy(m => m.Id)
           .ToListAsync();

    // Get by id
    public Task<Meetingroom?> GetById(int id) =>
        _db.Meetingrooms
           .AsNoTracking()
           .FirstOrDefaultAsync(m => m.Id == id);

    // Create
    public async Task<Meetingroom> Add(Meetingroom meetingroom)
    {
        _db.Meetingrooms.Add(meetingroom);
        await _db.SaveChangesAsync();
        return meetingroom;
    }

    // Update
    public async Task<Meetingroom> Update(Meetingroom meetingroom)
    {
        _db.Meetingrooms.Update(meetingroom);
        await _db.SaveChangesAsync();
        return meetingroom;
    }

    // Delete
    public async Task<bool> Delete(int id)
    {
        var entity = await _db.Meetingrooms.FindAsync(id);
        if (entity is null) return false;

        _db.Meetingrooms.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }


}
