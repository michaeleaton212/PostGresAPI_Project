using System.Text.Json.Serialization;

namespace PostGresAPI.Contracts
{
    public record BedroomDto(
        [property: JsonPropertyName("id")] int Id, 
 [property: JsonPropertyName("name")] string Name, 
        [property: JsonPropertyName("numberOfBeds")] int NumberOfBeds
    );
    
    public record CreateBedroomDto(
        [property: JsonPropertyName("name")] string Name, 
        [property: JsonPropertyName("numberOfBeds")] int NumberOfBeds
    );
    
    public record UpdateBedroomDto(
        [property: JsonPropertyName("name")] string Name, 
        [property: JsonPropertyName("numberOfBeds")] int NumberOfBeds
    );
}

// record is an immutable data structure for objects that only contain data
