namespace PostGresAPI.Contracts;

public record BookingDto(int Id, int RoomId, DateTimeOffset StartTime, DateTimeOffset EndTime, string? Title);

// Controller expects these DTOs when creating or updating a booking
public record CreateBookingDto(int RoomId, DateTimeOffset StartUtc, DateTimeOffset EndUtc, string? Title);
public record UpdateBookingDto(DateTimeOffset StartUtc, DateTimeOffset EndUtc, string? Title);

// recoord is an not changable datastrucure for objects that only contain data 