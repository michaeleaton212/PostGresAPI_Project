using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.Repository
{
    public interface IBedroomRepository
    {
        // Get all
        Task<List<Bedroom>> GetAll();
        // Get by Id
        Task<Bedroom?> GetById(int id);
        // Create
        Task<Bedroom> Add(CreateBedroomDto createBedroomDto);
        // Update
        Task<Bedroom?> Update(int id, UpdateBedroomDto dto);
        // Check if Bedroom exists
        Task<bool> ExistsAsync(int id);
        // Delete
        Task<bool> Delete(int id);
    }
}

// The interface tells my repository what it has to offer, but not how