namespace PostGresAPI.Contracts;

public record UserDto(int Id, string UserName, string Email);
public record CreateUserDto(string UserName, string Email);
public record UpdateUserDto(string UserName, string Email);
