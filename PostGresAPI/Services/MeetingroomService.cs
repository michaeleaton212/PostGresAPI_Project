using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services;

public class MeetingroomService : IMeetingroomService
{
    private readonly IMeetingroomRepository _repo;
    public MeetingroomService(IMeetingroomRepository repo) => _repo = repo; //Constructor Injection: the object must implement interface

    public Task<List<Meetingroom>> GetAll() => _repo.GetAll();
    public Task<Meetingroom?> GetById(int id) => _repo.GetById(id);

    public Task<Meetingroom> Create(string name, int numberOfChairs)
        => _repo.Add(new Meetingroom(name, numberOfChairs));

    public Task<Meetingroom?> Update(int id, string name, int numberOfChairs)
     => _repo.Update(id, name, numberOfChairs);


    public Task<bool> Delete(int id) => _repo.Delete(id);
}
