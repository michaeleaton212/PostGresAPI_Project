namespace PostGresAPI.Contracts;

public record BedroomDto(int Id, string Name, int NumberOfBeds);
public record CreateBedroomDto(string Name, int NumberOfBeds);
public record UpdateBedroomDto(string Name, int NumberOfBeds);
