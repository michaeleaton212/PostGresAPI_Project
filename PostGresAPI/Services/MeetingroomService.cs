using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services;

public class MeetingroomService
{
    private readonly MeetingroomRepository _repo;
    public MeetingroomService(MeetingroomRepository repo) => _repo = repo; // Constructor Injection

    public Task<List<Meetingroom>> GetAll() => _repo.GetAll();
    public Task<Meetingroom?> GetById(int id) => _repo.GetById(id);

    public Task<Meetingroom> Create(string name, int numberOfChairs)
        => _repo.Add(new Meetingroom(name, numberOfChairs));

    public async Task<Meetingroom?> Update(int id, string name, int numberOfChairs)
    {
        var entity = await _repo.GetById(id);
        if (entity is null) return null;

        entity.SetName(name);              
        entity.NumberOfChairs = numberOfChairs;

        return await _repo.Update(entity);
    }

    public Task<bool> Delete(int id) => _repo.Delete(id);
}
