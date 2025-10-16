namespace PostGresAPI.Models;

public sealed class Booking
{
    private Booking() { } // für EF

    public Booking(int roomId, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title = null)
    {
        RoomId = roomId;
        StartTime = startUtc;
        EndTime = endUtc;
        Title = title;
    }

    public int Id { get; private set; }
    public int RoomId { get; private set; }
    public Room Room { get; private set; } = null!;

    // vom Service änderbar
    public DateTimeOffset StartTime { get; internal set; }
    public DateTimeOffset EndTime { get; internal set; }
    public string? Title { get; internal set; }
}
