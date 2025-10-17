using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo; // field in that object from type IUserRepository gets saved in

        public UserService(IUserRepository repo) => _repo = repo; // Constructor Injection: the object must implement interface

        // Read
        public Task<List<User>> GetAll() => _repo.GetAll();
        public Task<User?> GetById(int id) => _repo.GetById(id);

        // Create
        public async Task<User> Create(string userName, string email, string? phone = null)
        {
            var u = new User(userName, email, phone ?? "");
            return await _repo.Add(u);
        }

        // Update 
        public Task<User?> Update(int id, string userName, string email, string? phone = null)
      => _repo.Update(id, userName, email, phone ?? "");


        // Delete
        public Task<bool> Delete(int id) => _repo.Delete(id);
    }
}
