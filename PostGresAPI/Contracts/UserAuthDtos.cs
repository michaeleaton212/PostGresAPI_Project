namespace PostGresAPI.Contracts;

public record UserRegisterDto(string UserName, string Email, string Phone, string Password);
public record UserLoginDto(string UserNameOrEmail, string Password);
public record UserAuthResultDto(int Id, string UserName, string Email);
