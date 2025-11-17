using System.Text.Json.Serialization;

namespace PostGresAPI.Contracts
{
    public record MeetingroomDto(
    [property: JsonPropertyName("id")] int Id, 
        [property: JsonPropertyName("name")] string Name, 
        [property: JsonPropertyName("numberOfChairs")] int NumberOfChairs
    );
    
    public record CreateMeetingroomDto(
        [property: JsonPropertyName("name")] string Name, 
      [property: JsonPropertyName("numberOfChairs")] int NumberOfChairs
    );
    
    public record UpdateMeetingroomDto(
        [property: JsonPropertyName("name")] string Name, 
[property: JsonPropertyName("numberOfChairs")] int NumberOfChairs
    );
}

// record is an immutable data structure for objects that only contain data
