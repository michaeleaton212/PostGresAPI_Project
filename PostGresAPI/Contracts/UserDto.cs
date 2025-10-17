namespace PostGresAPI.Contracts;

public record UserDto(int Id, string UserName, string Email); // define the fields returned to the client
public record CreateUserDto(string UserName, string Email); // define the fields required for creating a user
public record UpdateUserDto(string UserName, string Email); //define the fields that can be updated

// recoord is an not changable datastrucure for objects that only contain data 