namespace PostGresAPI.Contracts;

public record class MeetingroomDto
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int NumberOfChairs { get; init; }
    public string? ImagePath { get; init; }
    public MeetingroomDto(int id, string name, int numberOfChairs, string? imagePath)
    {
        Id = id;
        Name = name;
        NumberOfChairs = numberOfChairs;
        ImagePath = imagePath;
    }
    public MeetingroomDto() { }
}

public record class CreateMeetingroomDto
{
    public string Name { get; init; } = "";
    public int NumberOfChairs { get; init; }
    public string? ImagePath { get; init; }
}

public record class UpdateMeetingroomDto
{
    public string Name { get; init; } = "";
    public int NumberOfChairs { get; init; }
    public string? ImagePath { get; init; }
}

// record is an immutable data structure for objects that only contain data