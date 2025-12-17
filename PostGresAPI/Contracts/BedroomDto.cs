namespace PostGresAPI.Contracts;

public record class BedroomDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int NumberOfBeds { get; init; }
    public string? ImagePath { get; init; }
    public decimal PricePerNight { get; init; }
}

public record class CreateBedroomDto
{
    public string Name { get; init; } = "";
    public int NumberOfBeds { get; init; }
    public string? ImagePath { get; init; }
    public decimal PricePerNight { get; init; }
}

public record class UpdateBedroomDto
{
    public string Name { get; init; } = "";
    public int NumberOfBeds { get; init; }
    public string? ImagePath { get; init; }
    public decimal PricePerNight { get; init; }
}


// record is an immutable data structure for objects that only contain data


//dtos defines the structure of the data that is sent over the network
//defines the 