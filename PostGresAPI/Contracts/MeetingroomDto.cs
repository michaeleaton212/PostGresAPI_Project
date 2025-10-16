namespace PostGresAPI.Contracts;

public record MeetingroomDto(int Id, string Name, int NumberOfChairs);
public record CreateMeetingroomDto(string Name, int NumberOfChairs);
public record UpdateMeetingroomDto(string Name, int NumberOfChairs);
