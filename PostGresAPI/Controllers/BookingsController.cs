using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts; // BookingDto, CreateBookingDto, UpdateBookingDto, LoginRequestDto, LoginResponseDto
using PostGresAPI.Services;
using PostGresAPI.Auth;    // ITokenService

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _svc;
    private readonly ITokenService _tokens;

    public BookingsController(IBookingService svc, ITokenService tokens)
    {
        _svc = svc;
        _tokens = tokens;
    }

    // GET /api/bookings 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
    {
        // Service liefert bereits BookingDto -> kein erneutes Mapping nötig
        var dtos = await _svc.GetAll();
        return Ok(dtos);
    }

    // GET /api/bookings/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        var dto = await _svc.GetById(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    // POST /api/bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingDto dto)
    {
        var (ok, err, result) = await _svc.Create(dto);
        if (!ok) return BadRequest(new { error = err });

        var created = result!;
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/bookings/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingDto>> Update(int id, UpdateBookingDto dto)
    {
        var (ok, err, result) = await _svc.Update(id, dto);
        if (!ok) return BadRequest(new { error = err });

        return Ok(result!);
    }

    // DELETE /api/bookings/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (ok, err) = await _svc.Delete(id);
        return ok ? NoContent() : BadRequest(new { error = err });
    }


    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var bookingId = await _svc.GetBookingIdByCredentials(dto.BookingNumber, dto.Name);
        if (bookingId is null)
            return Unauthorized(new { error = "Ungültige Kombination aus Buchungsnummer und Name." });

        var token = _tokens.Create(bookingId.Value, DateTimeOffset.UtcNow.AddMinutes(30)); //set time
        return Ok(new LoginResponseDto(bookingId.Value, token));
    }

    [HttpGet("{bookingId:int}/secure")]
    public async Task<ActionResult<BookingDto>> GetSecure(
        int bookingId,
        [FromQuery] string? token,
        [FromHeader(Name = "X-Login-Token")] string? tokenHeader)
    {
        var t = tokenHeader ?? token;
        if (string.IsNullOrWhiteSpace(t)) return Unauthorized();

        if (!_tokens.TryValidate(t, out var tokenBookingId) || tokenBookingId != bookingId)
            return Unauthorized();

        var dto = await _svc.GetById(bookingId);
        return dto is null ? NotFound() : Ok(dto);
    }
}
