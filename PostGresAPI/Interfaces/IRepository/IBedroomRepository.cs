using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Repository
{
    public interface IBedroomRepository
    {
        // Get all
        Task<List<Bedroom>> GetAll();

        // Get by Id
        Task<Bedroom?> GetById(int id);

        // Create
        Task<Bedroom> Add(Bedroom bedroom);

        // Update
        Task<Bedroom> Update(Bedroom bedroom);

        // Check if Bedroom exists
        Task<bool> ExistsAsync(int id);

        // Delete
        Task<bool> Delete(int id);
    }
}
