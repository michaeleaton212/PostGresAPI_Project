using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Repository
{
    public interface IRoomRepository
    {
        // Get all
        Task<List<Room>> GetAll();

        // Get Room by id
        Task<Room?> GetById(int id);

        // Get Meetingrooms
        Task<List<Meetingroom>> GetMeetingrooms();

        // Get Bedrooms
        Task<List<Bedroom>> GetBedrooms();

        // Creates a new Room
        Task<Room> Add(Room room);

        // Updates an already existing Room
        Task<Room?> UpdateName(int id, string name);

        // Delete Room by Id
        Task<bool> Delete(int id);

        // Check if Room exists
        Task<bool> Exists(int id);
    }
}

// The interface tells my repository what it has to offer