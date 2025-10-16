using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Services
{
    public interface IMeetingroomService
    {
        Task<List<Meetingroom>> GetAll();
        Task<Meetingroom?> GetById(int id);
        Task<Meetingroom> Create(string name, int numberOfChairs);
        Task<Meetingroom?> Update(int id, string name, int numberOfChairs);
        Task<bool> Delete(int id);
    }
}