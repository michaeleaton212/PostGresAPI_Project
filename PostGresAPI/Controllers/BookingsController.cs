using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts;   // BookingDto, CreateBookingDto, UpdateBookingDto
using PostGresAPI.Services;

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly BookingService _svc;

    public BookingsController(BookingService svc)
    {
        _svc = svc;
    }

    // GET /api/bookings 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
    {
        var items = await _svc.GetAll();
        var dtos = items.Select(b => new BookingDto(b.Id, b.RoomId, b.StartTime, b.EndTime, b.Title));
        return Ok(dtos);
    }

    

    // GET /api/bookings/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        var b = await _svc.GetById(id);
        if (b is null) return NotFound();
        return Ok(new BookingDto(b.Id, b.RoomId, b.StartTime, b.EndTime, b.Title));
    }

    // POST /api/bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingDto dto)
    {
        var (ok, err, result) = await _svc.Create(dto.RoomId, dto.StartUtc, dto.EndUtc, dto.Title);
        if (!ok) return BadRequest(new { error = err });

        var b = result!;
        var read = new BookingDto(b.Id, b.RoomId, b.StartTime, b.EndTime, b.Title);
        return CreatedAtAction(nameof(GetById), new { id = b.Id }, read);
    }

    // PUT /api/bookings/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingDto>> Update(int id, UpdateBookingDto dto)
    {
        var (ok, err, result) = await _svc.Update(id, dto.StartUtc, dto.EndUtc, dto.Title);
        if (!ok) return BadRequest(new { error = err });

        var b = result!;
        return Ok(new BookingDto(b.Id, b.RoomId, b.StartTime, b.EndTime, b.Title));
    }

    // DELETE /api/bookings/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (ok, err) = await _svc.Delete(id);
        return ok ? NoContent() : BadRequest(new { error = err });
    }
}
