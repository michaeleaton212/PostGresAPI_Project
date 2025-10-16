using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Services
{
    public interface IUserService
    {
        // Read
        Task<List<User>> GetAll();
        Task<User?> GetById(int id);

        // Create
        Task<User> Create(string userName, string email, string? phone = null);

        // Update
        Task<User?> Update(int id, string userName, string email, string? phone = null);

        // Delete
        Task<bool> Delete(int id);
    }
}
