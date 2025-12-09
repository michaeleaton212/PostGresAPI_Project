namespace PostGresAPI.Models;

public sealed class Booking
{
    private Booking() { } // für EF

    public Booking(int roomId, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title = null, string? bookingNumber = null)
    {
        RoomId = roomId;
        StartTime = startUtc;
        EndTime = endUtc;
        Title = title;
        BookingNumber = bookingNumber ?? Guid.NewGuid().ToString()[..8]; // Default:8-stelliger Code
        Status = BookingStatus.Pending; // Default status
    }

    public int Id { get; private set; }
    public int RoomId { get; private set; }
    public Room Room { get; private set; } = null!;

    // vom Service änderbar
    public DateTimeOffset StartTime { get; internal set; }
    public DateTimeOffset EndTime { get; internal set; }
    public string? Title { get; internal set; }

    // NEU: öffentliche Buchungsnummer
    public string BookingNumber { get; private set; } = null!;

    // NEU: Booking Status
    public BookingStatus Status { get; internal set; }
}
