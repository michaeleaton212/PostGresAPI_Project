using PostGresAPI.Contracts;
using PostGresAPI.Models;

namespace PostGresAPI.Extensions;

public static class UserMappingExtensions
{
    // Entity to DTO
    public static UserDto ToDto(this User user)
    {
        return new UserDto(user.Id, user.UserName, user.Email);
    }

    // CreateUserDto to Entity
    public static User ToEntity(this CreateUserDto dto)
    {
        return new User(dto.UserName, dto.Email);
    }

    // UpdateUserDto to Entity
    public static void UpdateEntity(this UpdateUserDto dto, User user)
    {
        user.UserName = dto.UserName;
        user.Email = dto.Email;
    }

}
//here in the file we define extension methods for mapping between User entities and their corresponding DTOs (Data Transfer Objects).