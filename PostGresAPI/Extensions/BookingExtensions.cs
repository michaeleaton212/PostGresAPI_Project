using System;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.Extensions;

public static class BookingMappingExtensions
{
    // Entity to BookingDto
    public static BookingDto ToDto(this Booking b)
        => new(b.Id, b.RoomId, b.StartTime, b.EndTime, b.Title, b.BookingNumber, b.Status.ToString());


    // CreateBookingDto to Entity
    public static Booking ToEntity(this CreateBookingDto dto)
    {
        // Generiere eine eindeutige8-stellige Buchungsnummer
        var bookingNumber = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return new Booking(dto.RoomId, dto.StartUtc, dto.EndUtc, dto.Title, bookingNumber);
    }


    // UpdateBookingDto to Entity
    public static void UpdateEntity(this UpdateBookingDto dto, Booking entity)
    {
        entity.StartTime = dto.StartUtc;
        entity.EndTime = dto.EndUtc;
        entity.Title = dto.Title;
    }


    // Apply updates to Booking entity
    public static void ApplyUpdate(this Booking entity, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title)
    {
        entity.StartTime = startUtc;
        entity.EndTime = endUtc;
        entity.Title = title;
    }

    // Apply status update to Booking entity
    public static void ApplyStatusUpdate(this Booking entity, BookingStatus status)
    {
        entity.Status = status;
    }
}
