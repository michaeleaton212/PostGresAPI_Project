using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Repository
{
    public interface IUserRepository
    {
        // Read all users
        Task<List<User>> GetAll();

        // Get by id
        Task<User?> GetById(int id);

        // Create
        Task<User> Add(User user);

        // Update
        Task<User> Update(User user);

        // Delete
        Task<bool> Delete(int id);
    }
}
