using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Repository
{
    public interface IMeetingroomRepository
    {
        // Get all 
        Task<List<Meetingroom>> GetAll();

        // Get by id
        Task<Meetingroom?> GetById(int id);

        // Create
        Task<Meetingroom> Add(Meetingroom meetingroom);

        // Update
        Task<Meetingroom> Update(Meetingroom meetingroom);

        // Delete
        Task<bool> Delete(int id);
    }
}
