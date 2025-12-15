using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Contracts;
using PostGresAPI.Extensions;

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

            return rooms.Select(r => r.ToDto()).ToList();

        }

        public async Task<BedroomDto?> GetById(int id)
        {
            var room = await _repo.GetById(id);
            return room?.ToDto();
        }

        // Create
        public async Task<BedroomDto> Create(CreateBedroomDto createBedroomDto)
        {
            var created = await _repo.Add(createBedroomDto);
            return created.ToDto();
        }

        // Update
        public async Task<BedroomDto?> Update(int id, UpdateBedroomDto updateBedroomDto)
        {
            var updated = await _repo.Update(id, updateBedroomDto);
            return updated is null ? null : updated.ToDto();
        }

        // Delete
        public Task<bool> Delete(int id) => _repo.Delete(id);
    }
}
// The service contains the business logic and interacts with the repository