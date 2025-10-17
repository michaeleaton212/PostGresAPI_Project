using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Contracts;

namespace PostGresAPI.Services
{
    public interface IMeetingroomService
    {
        Task<List<MeetingroomDto>> GetAll();
        Task<MeetingroomDto?> GetById(int id);
        Task<MeetingroomDto> Create(string name, int numberOfChairs);
        Task<MeetingroomDto?> Update(int id, string name, int numberOfChairs);
        Task<bool> Delete(int id);
    }
}
