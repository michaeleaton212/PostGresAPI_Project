using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Contracts;

namespace PostGresAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo) => _repo = repo;

        // Read
        public async Task<List<UserDto>> GetAll()
        {
            var users = await _repo.GetAll();
            return users.Select(u => new UserDto(u.Id, u.UserName, u.Email)).ToList();
        }

        public async Task<UserDto?> GetById(int id)
        {
            var user = await _repo.GetById(id);
            return user is null ? null : new UserDto(user.Id, user.UserName, user.Email); // build a new UserDto from user entity, thats caled mapping
        }

        // Create
        public async Task<UserDto> Create(string userName, string email, string? phone = null)
        {
            var createUserDto = new CreateUserDto(userName, email, phone ?? "");
            var createdUser = await _repo.Add(createUserDto);
            return new UserDto(createdUser.Id, createdUser.UserName, createdUser.Email);
        }

        // Update
        public async Task<UserDto?> Update(int id, string userName, string email, string? phone = null)
        {
            var updatedUser = await _repo.Update(id, userName, email, phone ?? "");
            if (updatedUser is null)
                return null;

            return new UserDto(updatedUser.Id, updatedUser.UserName, updatedUser.Email);
        }

        // Delete
        public Task<bool> Delete(int id) => _repo.Delete(id);
    }
}
