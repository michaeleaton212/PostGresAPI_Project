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

    // Helper method to validate token and extract booking IDs
    private bool TryGetAuthorizedBookingIds(out List<int> bookingIds)
    {
        bookingIds = new List<int>();
        
        // Get token from header
        var token = Request.Headers["X-Login-Token"].FirstOrDefault()
                 ?? Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        
        if (string.IsNullOrWhiteSpace(token))
            return false;
        
        return _tokens.TryValidate(token, out bookingIds);
    }

    // GET /api/bookings 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
    {
        // This endpoint should be protected - only return user's bookings
        if (!TryGetAuthorizedBookingIds(out var authorizedIds))
            return Unauthorized(new { error = "Ungültiger oder fehlender Token." });
        
        var allDtos = await _svc.GetByIds(authorizedIds);
        return Ok(allDtos);
    }

    // GET /api/bookings/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        // Validate token and check if user is authorized for this booking
        if (!TryGetAuthorizedBookingIds(out var authorizedIds))
            return Unauthorized(new { error = "Ungültiger oder fehlender Token." });
        
        if (!authorizedIds.Contains(id))
            return Forbid();
        
        var dto = await _svc.GetById(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    // GET /api/bookings/room/{roomId}
    [HttpGet("room/{roomId:int}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByRoomId(int roomId)
    {
        // This endpoint should return all bookings for a room (for availability)
        // but filtered by authorized bookings
        if (!TryGetAuthorizedBookingIds(out var authorizedIds))
            return Unauthorized(new { error = "Ungültiger oder fehlender Token." });
        
        var allDtos = await _svc.GetByRoomId(roomId);
        // Only return bookings that the user is authorized to see
        var filtered = allDtos.Where(b => authorizedIds.Contains(b.Id)).ToList();
        return Ok(filtered);
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
        // Validate token and check if user is authorized for this booking
        if (!TryGetAuthorizedBookingIds(out var authorizedIds))
            return Unauthorized(new { error = "Ungültiger oder fehlender Token." });
        
        if (!authorizedIds.Contains(id))
            return Forbid();
        
        var (ok, err, result) = await _svc.Update(id, dto);
        if (!ok) return BadRequest(new { error = err });

        return Ok(result!);
    }

    // PATCH /api/bookings/{id}/status
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<BookingDto>> UpdateStatus(int id, UpdateBookingStatusDto dto)
    {
        // Validate token and check if user is authorized for this booking
        if (!TryGetAuthorizedBookingIds(out var authorizedIds))
            return Unauthorized(new { error = "Ungültiger oder fehlender Token." });
        
        if (!authorizedIds.Contains(id))
            return Forbid();
        
        var (ok, err, result) = await _svc.UpdateStatus(id, dto);
        if (!ok) return BadRequest(new { error = err });

        return Ok(result!);
    }

    // DELETE /api/bookings/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Validate token and check if user is authorized for this booking
        if (!TryGetAuthorizedBookingIds(out var authorizedIds))
            return Unauthorized(new { error = "Ungültiger oder fehlender Token." });
        
        if (!authorizedIds.Contains(id))
            return Forbid();
        
        var (ok, err) = await _svc.Delete(id);
        return ok ? NoContent() : BadRequest(new { error = err });
    }


    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var bookingIds = await _svc.GetBookingIdsByCredentials(dto.BookingNumber, dto.Name);
        if (bookingIds == null || bookingIds.Count == 0)
            return Unauthorized(new { error = "Ungültige Kombination aus Buchungsnummer und Name." });

        var token = _tokens.Create(bookingIds, DateTimeOffset.UtcNow.AddMinutes(30));
        return Ok(new LoginResponseDto(bookingIds, token));
    }

    [HttpGet("{bookingId:int}/secure")]
    public async Task<ActionResult<BookingDto>> GetSecure(
        int bookingId,
        [FromQuery] string? token,
        [FromHeader(Name = "X-Login-Token")] string? tokenHeader)
    {
        var t = tokenHeader ?? token;
        if (string.IsNullOrWhiteSpace(t)) return Unauthorized();

        if (!_tokens.TryValidate(t, out var tokenBookingIds) || !tokenBookingIds.Contains(bookingId))
            return Unauthorized();

        var dto = await _svc.GetById(bookingId);
        return dto is null ? NotFound() : Ok(dto);
    }

    // GET /api/bookings/by-name/{name}
    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByName(string name)
    {
        // This endpoint should be protected
        if (!TryGetAuthorizedBookingIds(out var authorizedIds))
            return Unauthorized(new { error = "Ungültiger oder fehlender Token." });
        
        var allDtos = await _svc.GetByName(name);
        // Only return bookings that the user is authorized to see
        var filtered = allDtos.Where(b => authorizedIds.Contains(b.Id)).ToList();
        return Ok(filtered);
    }

    // POST /api/bookings/by-ids
    [HttpPost("by-ids")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByIds([FromBody] List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return BadRequest(new { error = "Booking IDs list cannot be empty." });

        // Validate token and check if user is authorized for all requested bookings
        if (!TryGetAuthorizedBookingIds(out var authorizedIds))
            return Unauthorized(new { error = "Ungültiger oder fehlender Token." });
        
        // Only return bookings that the user is authorized to see
        var requestedIds = ids.Where(id => authorizedIds.Contains(id)).ToList();
        
        if (requestedIds.Count == 0)
            return Forbid();
        
        var dtos = await _svc.GetByIds(requestedIds);
        return Ok(dtos);
    }
}
