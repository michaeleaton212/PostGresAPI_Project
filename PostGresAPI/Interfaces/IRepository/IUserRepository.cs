using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Models;

namespace PostGresAPI.Repository
{
    public interface IUserRepository
    {
        // Read all users
        Task<List<User>> GetAll(); //tells repo that it has to return a list od users

        // Get by id
        Task<User?> GetById(int id); //tells repository that is has to return a user with id  or null if not found

        // Create
        Task<User> Add(User user);

        // Update
        Task<User?> Update(int id, string userName, string email, string phone);

        // Delete
        Task<bool> Delete(int id);
    }
}

// The interface tells my repository what it has to offer