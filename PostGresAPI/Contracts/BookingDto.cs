namespace PostGresAPI.Contracts
{
    public record BookingDto(int Id, int RoomId, DateTimeOffset StartTime, DateTimeOffset EndTime, string? Title, string BookingNumber, string Status, int? UserId, int NumberOfPersons);

    // Controller expects these DTOs when creating or updating a booking
    public record CreateBookingDto(int RoomId, DateTimeOffset StartUtc, DateTimeOffset EndUtc, string? Title, int? UserId = null, int NumberOfPersons = 1);
    public record UpdateBookingDto(DateTimeOffset StartUtc, DateTimeOffset EndUtc, string? Title, int NumberOfPersons);
    public record UpdateBookingStatusDto(string Status);

    // recoord is an not changable datastrucure for objects that only contain data  
}
