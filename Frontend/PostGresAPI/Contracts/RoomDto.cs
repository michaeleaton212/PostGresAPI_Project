using System.Text.Json.Serialization;

namespace PostGresAPI.Contracts;

public record RoomDto(
    [property: JsonPropertyName("id")] int Id, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("type")] string Type, 
    [property: JsonPropertyName("numberOfBeds")] int? NumberOfBeds, 
    [property: JsonPropertyName("numberOfChairs")] int? NumberOfChairs
);

public record CreateRoomDto(string Name, string Type);
public record UpdateRoomDto(string Name, string Type);
public record UpdateRoomNameDto(string Name);
