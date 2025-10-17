using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services;

public class BedroomService : IBedroomService
{
    private readonly IBedroomRepository _repo;
    public BedroomService(IBedroomRepository repo) => _repo = repo;// Constructor Injection

    public Task<List<Bedroom>> GetAll() => _repo.GetAll();
    public Task<Bedroom?> GetById(int id) => _repo.GetById(id);

    public Task<Bedroom> Create(string name, int numberOfBeds)
        => _repo.Add(new Bedroom(name, numberOfBeds));

    public Task<Bedroom?> Update(int id, string name, int numberOfBeds)
        => _repo.Update(id, name, numberOfBeds);


    public Task<bool> Delete(int id) => _repo.Delete(id);
}