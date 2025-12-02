using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Contracts;
using PostGresAPI.Extensions;

namespace PostGresAPI.Services
{
    public class MeetingroomService : IMeetingroomService
    {
        private readonly IMeetingroomRepository _repo;
        public MeetingroomService(IMeetingroomRepository repo) => _repo = repo;

        // Read
        public async Task<List<MeetingroomDto>> GetAll()
            => (await _repo.GetAll()).Select(m => m.ToDto()).ToList();

        public async Task<MeetingroomDto?> GetById(int id)
            => (await _repo.GetById(id)) is { } m ? m.ToDto() : null;

        // Create
        public async Task<MeetingroomDto> Create(CreateMeetingroomDto createMeetingroomDto)
        {
            var created = await _repo.Add(createMeetingroomDto);
            return created.ToDto();
        }

        // Update
        public async Task<MeetingroomDto?> Update(int id, UpdateMeetingroomDto updateMeetingroomDto)
        {
            var updated = await _repo.Update(id, updateMeetingroomDto);
            return updated is null ? null : updated.ToDto();
        }

        // Delete
        public Task<bool> Delete(int id) => _repo.Delete(id);
    }
}
