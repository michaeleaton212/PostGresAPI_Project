using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services
{
    public class UserService
    {
        private readonly UserRepository _repo;

        public UserService(UserRepository repo) => _repo = repo; // Constructor Injection

        // Read
        public Task<List<User>> GetAll() => _repo.GetAll();
        public Task<User?> GetById(int id) => _repo.GetById(id);

        // Create
        public async Task<User> Create(string userName, string email, string? phone = null)
        {


            var u = new User(userName, email, phone ?? ""); // here to give value over to constructor
            return await _repo.Add(u);
        }

        // Update 
        public async Task<User?> Update(int id, string userName, string email, string? phone = null)
        {
            var entity = await _repo.GetById(id);
            if (entity is null) return null;

            
            entity.Apply(userName, email, phone); // here to give value over to method Apply
            return await _repo.Update(entity);
            

        }

        // Delete
        public Task<bool> Delete(int id) => _repo.Delete(id);
    }
}
