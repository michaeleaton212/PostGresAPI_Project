namespace PostGresAPI.Contracts;

public record RoomDto(int Id, string Name, string Type);
public record UpdateRoomNameDto(string Name);

// recoord is an not changable datastrucure for objects that only contain data 