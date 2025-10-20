using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Contracts;

namespace PostGresAPI.Services
{
    public class MeetingroomService : IMeetingroomService
    {
        private readonly IMeetingroomRepository _repo;
        public MeetingroomService(IMeetingroomRepository repo) => _repo = repo;

        // mapping
        private static MeetingroomDto ToDto(Meetingroom m)
            => new MeetingroomDto(m.Id, m.Name, m.NumberOfChairs);

        // Read
        public async Task<List<MeetingroomDto>> GetAll()
            => (await _repo.GetAll()).Select(ToDto).ToList();

        public async Task<MeetingroomDto?> GetById(int id)
            => (await _repo.GetById(id)) is { } m ? ToDto(m) : null;

        // Create
        public async Task<MeetingroomDto> Create(string name, int numberOfChairs)
        {
            var dto = new CreateMeetingroomDto(name, numberOfChairs);
            var created = await _repo.Add(dto); // 
            return ToDto(created);
        }


        // Update
        public async Task<MeetingroomDto?> Update(int id, string name, int numberOfChairs)
        {
            var updated = await _repo.Update(id, name, numberOfChairs);
            return updated is null ? null : ToDto(updated);
        }

        // Delete
        public Task<bool> Delete(int id) => _repo.Delete(id);
    }
}
