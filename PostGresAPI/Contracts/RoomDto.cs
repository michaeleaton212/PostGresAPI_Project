namespace PostGresAPI.Contracts;

public record class RoomDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Type { get; init; } = "";
    public int? NumberOfBeds { get; init; }
    public int? NumberOfChairs { get; init; }
    public string? image { get; init; }
}
