using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Contracts;

namespace PostGresAPI.Services
{
    public class BedroomService : IBedroomService
    {
        private readonly IBedroomRepository _repo;
        public BedroomService(IBedroomRepository repo) => _repo = repo; // Constructor Injection

        // Read
        public async Task<List<BedroomDto>> GetAll()
        {
            var rooms = await _repo.GetAll();
            return rooms.Select(r => new BedroomDto(r.Id, r.Name, r.NumberOfBeds)).ToList();
        }

        public async Task<BedroomDto?> GetById(int id)
        {
            var room = await _repo.GetById(id);
            return room is null ? null : new BedroomDto(room.Id, room.Name, room.NumberOfBeds);
        }

        // Create
        public async Task<BedroomDto> Create(string name, int numberOfBeds)
        {
            var entity = new Bedroom(name, numberOfBeds);
            var created = await _repo.Add(entity);
            return new BedroomDto(created.Id, created.Name, created.NumberOfBeds);
        }

        // Update
        public async Task<BedroomDto?> Update(int id, string name, int numberOfBeds)
        {
            var updated = await _repo.Update(id, name, numberOfBeds);
            if (updated is null) return null;
            return new BedroomDto(updated.Id, updated.Name, updated.NumberOfBeds);
        }

        // Delete
        public Task<bool> Delete(int id) => _repo.Delete(id);
    }
}
