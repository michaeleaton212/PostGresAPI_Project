using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Services
{
    public interface IRoomService
    {
        // Read
        Task<List<Room>> GetAll();
        Task<Room?> GetById(int id);
        Task<List<Meetingroom>> GetMeetingrooms();
        Task<List<Bedroom>> GetBedrooms();

        // Create
        Task<Room> Create(Room room);

        // Update
        Task<Room?> UpdateName(int id, string name);

        // Delete
        Task<bool> Delete(int id);
    }
}
