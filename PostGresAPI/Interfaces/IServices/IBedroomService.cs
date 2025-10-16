using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Services
{
    public interface IBedroomService
    {
        Task<List<Bedroom>> GetAll();
        Task<Bedroom?> GetById(int id);
        Task<Bedroom> Create(string name, int numberOfBeds);
        Task<Bedroom?> Update(int id, string name, int numberOfBeds);
        Task<bool> Delete(int id);
    }
}