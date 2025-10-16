using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _repo;

    public RoomService(IRoomRepository repo) => _repo = repo; // Constructor Injection

    // Read
    public Task<List<Room>> GetAll() => _repo.GetAll();
    public Task<Room?> GetById(int id) => _repo.GetById(id);
    public Task<List<Meetingroom>> GetMeetingrooms() => _repo.GetMeetingrooms();
    public Task<List<Bedroom>> GetBedrooms() => _repo.GetBedrooms();

    // Create
    public Task<Room> Create(Room room) => _repo.Add(room);

    //Update
    public async Task<Room?> UpdateName(int id, string name)
    {
        var entity = await _repo.GetById(id);
        if (entity is null) return null;


        entity.SetName(name);               // nur Zuweisung (keine Logik in Entity)
        return await _repo.Update(entity);
    }

    // Delete
    public Task<bool> Delete(int id) => _repo.Delete(id);
}
