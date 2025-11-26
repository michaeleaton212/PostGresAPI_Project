using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts; // BookingDto, CreateBookingDto, UpdateBookingDto, LoginRequestDto, LoginResponseDto
using PostGresAPI.Services;

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _svc;

    public BookingsController(IBookingService svc)
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
        var (ok, err, result) = await _svc.Create(dto);
        if (!ok) return BadRequest(new { error = err });

        var b = result!;
        var read = new BookingDto(b.Id, b.RoomId, b.StartTime, b.EndTime, b.Title);
        return CreatedAtAction(nameof(GetById), new { id = b.Id }, read);
    }

    // PUT /api/bookings/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingDto>> Update(int id, UpdateBookingDto dto)
    {
        var (ok, err, result) = await _svc.Update(id, dto);
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

    //POST /api/bookings/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var roomId = await _svc.GetRoomIdByCredentials(dto.BookingNumber, dto.Name);
        if (roomId is null)
            return Unauthorized(new { error = "Ungültige Kombination aus Buchungsnummer und Name." });

        return Ok(new LoginResponseDto(roomId.Value));
    }
}
