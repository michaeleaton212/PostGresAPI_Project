using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookings;
    private readonly IRoomRepository _rooms;

    public BookingService(IBookingRepository bookings, IRoomRepository rooms) // Constructor Injection
    {
        _bookings = bookings;
        _rooms = rooms;
    }

    public Task<List<Booking>> GetAll() => _bookings.GetAll();
    public Task<Booking?> GetById(int id) => _bookings.GetById(id);

    public bool IsActive(Booking booking, DateTimeOffset atUtc)
        => booking.StartTime <= atUtc && atUtc < booking.EndTime;

    public async Task<(bool Ok, string? Error, Booking? Result)> Create(
        int roomId, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title)
    {
        if (startUtc >= endUtc)
            return (false, "Start must be before End.", null);

        if (!await _rooms.Exists(roomId))
            return (false, $"Room {roomId} not found.", null);

        if (await _bookings.HasOverlap(roomId, startUtc, endUtc))
            return (false, "Time range already booked.", null);

        var entity = new Booking(roomId, startUtc, endUtc, title);
        await _bookings.Add(entity);
        return (true, null, entity);
    }

    public async Task<(bool Ok, string? Error, Booking? Result)> Update(
        int id, DateTimeOffset startUtc, DateTimeOffset endUtc, string? title)
    {
        if (startUtc >= endUtc)
            return (false, "Start must be before End.", null);
        var entity = await _bookings.GetById(id);
        if (entity is null)
            return (false, "Booking not found.", null);
        var hasOverlap = await _bookings.HasOverlap(entity.RoomId, startUtc, endUtc, excludeBookingId: id);
        if (hasOverlap)
            return (false, "Time range already booked.", null);

        var updated = await _bookings.Update(id, startUtc, endUtc, title);
        return (true, null, updated);
    }


    public async Task<(bool Ok, string? Error)> Delete(int id)
    {
        var ok = await _bookings.Delete(id);
        return ok ? (true, null) : (false, "Booking not found.");
    }
}