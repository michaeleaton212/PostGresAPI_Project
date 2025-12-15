using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Contracts;

namespace PostGresAPI.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAll();
        Task<UserDto?> GetById(int id);
        Task<UserDto> Create(CreateUserDto createUserDto);
        Task<UserDto?> Update(int id, UpdateUserDto updateUserDto);
        Task<bool> Delete(int id);
    }
}


// The interface tells my service what it has to offer, but not how

