namespace PostGresAPI.Contracts;

public record RoomDto(int Id, string Name, string Type);
public record CreateRoomDto(string Name, string Type);
public record UpdateRoomDto(string Name, string Type);
public record UpdateRoomNameDto(string Name);