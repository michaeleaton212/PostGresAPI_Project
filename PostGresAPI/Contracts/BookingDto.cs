// Contracts/BookingDto.cs
namespace PostGresAPI.Contracts;

public record BookingDto(int Id, int RoomId, DateTimeOffset StartTime, DateTimeOffset EndTime, string? Title);

// Controller erwartet diese:
public record CreateBookingDto(int RoomId, DateTimeOffset StartUtc, DateTimeOffset EndUtc, string? Title);
public record UpdateBookingDto(DateTimeOffset StartUtc, DateTimeOffset EndUtc, string? Title);
