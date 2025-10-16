using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services;

public class BedroomService
{
    private readonly BedroomRepository _repo;
    public BedroomService(BedroomRepository repo) => _repo = repo;// Constructor Injection

    public Task<List<Bedroom>> GetAll() => _repo.GetAll();
    public Task<Bedroom?> GetById(int id) => _repo.GetById(id);

    public Task<Bedroom> Create(string name, int numberOfBeds)
        => _repo.Add(new Bedroom(name, numberOfBeds));

    public async Task<Bedroom?> Update(int id, string name, int numberOfBeds)
    {
        var entity = await _repo.GetById(id);
        if (entity is null) return null;

        entity.SetName(name);           
        entity.NumberOfBeds = numberOfBeds;

        return await _repo.Update(entity);
    }

    public Task<bool> Delete(int id) => _repo.Delete(id);
}
