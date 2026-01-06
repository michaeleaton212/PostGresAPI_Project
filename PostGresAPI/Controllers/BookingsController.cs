using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostGresAPI.Contracts;
using PostGresAPI.Services;
using PostGresAPI.Auth;

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _svc;
    private readonly ITokenService _tokens;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService svc, ITokenService tokens, ILogger<BookingsController> logger)
    {
        _svc = svc;
        _tokens = tokens;
        _logger = logger;
    }

    // Helper method to validate token and extract booking IDs
    private bool TryGetAuthorizedBookingIds(out List<int> bookingIds)
    {
        bookingIds = new List<int>();
        
        var token = Request.Headers["X-Login-Token"].FirstOrDefault()
                 ?? Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        
        if (string.IsNullOrWhiteSpace(token))
            return false;
        
        return _tokens.TryValidate(token, out bookingIds);
    }

    // Helper method to get user ID from session
    private int? GetUserIdFromSession()
    {
        var userIdStr = Request.Headers["X-User-Id"].FirstOrDefault();
        if (int.TryParse(userIdStr, out var userId))
            return userId;
        return null;
    }

    // GET /api/bookings 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
    {
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "Benutzer nicht angemeldet." });
        
        var allDtos = await _svc.GetByUserId(userId.Value);
        return Ok(allDtos);
    }

    // GET /api/bookings/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "Benutzer nicht angemeldet." });
        
        var dto = await _svc.GetById(id);
        if (dto is null) return NotFound();
        
        // Check if booking belongs to user
        if (dto.UserId != userId)
            return Forbid();
        
        return Ok(dto);
    }

    // GET /api/bookings/room/{roomId}
    // PUBLIC: Availability requires all bookings for the room, regardless of user
    [HttpGet("room/{roomId:int}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByRoomId(int roomId)
    {
        var allDtos = await _svc.GetByRoomId(roomId);
        return Ok(allDtos);
    }

    // POST /api/bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingDto dto)
    {
        _logger.LogInformation("=== BOOKING REQUEST RECEIVED ===");
        _logger.LogInformation("CreateBookingDto received - RoomId: {RoomId}, UserId from DTO: {UserId}", 
            dto.RoomId, dto.UserId?.ToString() ?? "NULL");

        var userId = GetUserIdFromSession();
        _logger.LogInformation("UserId from session header (X-User-Id): {UserId}", userId?.ToString() ?? "NULL");

        if (userId == null)
        {
            _logger.LogWarning("❌ No UserId in session header, user not logged in");
            return Unauthorized(new { error = "Benutzer nicht angemeldet." });
        }
        
        // Set userId in the DTO so the service can send email to the user
        var dtoWithUserId = dto with { UserId = userId };
        _logger.LogInformation("Updated DTO with UserId: {UserId}", dtoWithUserId.UserId);
        
        var (ok, err, result) = await _svc.Create(dtoWithUserId);
        if (!ok) 
        {
            _logger.LogWarning("Booking creation failed: {Error}", err);
            return BadRequest(new { error = err });
        }

        var created = result!;
        _logger.LogInformation("✅ Booking created successfully: Id={Id}, BookingNumber={BookingNumber}", 
            created.Id, created.BookingNumber);
        _logger.LogInformation("=== BOOKING REQUEST COMPLETE ===");
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/bookings/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingDto>> Update(int id, UpdateBookingDto dto)
    {
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "Benutzer nicht angemeldet." });
        
        var existing = await _svc.GetById(id);
        if (existing == null) return NotFound();
        
        if (existing.UserId != userId)
            return Forbid();
        
        var (ok, err, result) = await _svc.Update(id, dto);
        if (!ok) return BadRequest(new { error = err });

        return Ok(result!);
    }

    // PATCH /api/bookings/{id}/status
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<BookingDto>> UpdateStatus(int id, UpdateBookingStatusDto dto)
    {
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "Benutzer nicht angemeldet." });
        
        var existing = await _svc.GetById(id);
        if (existing == null) return NotFound();
        
        if (existing.UserId != userId)
            return Forbid();
        
        var (ok, err, result) = await _svc.UpdateStatus(id, dto);
        if (!ok) return BadRequest(new { error = err });

        return Ok(result!);
    }

    // DELETE /api/bookings/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "Benutzer nicht angemeldet." });
        
        var existing = await _svc.GetById(id);
        if (existing == null) return NotFound();
        
        if (existing.UserId != userId)
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
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "Benutzer nicht angemeldet." });
        
        var allDtos = await _svc.GetByName(name);
        var filtered = allDtos.Where(b => b.UserId == userId).ToList();
        return Ok(filtered);
    }

    // POST /api/bookings/by-ids
    [HttpPost("by-ids")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetByIds([FromBody] List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            return BadRequest(new { error = "Booking IDs list cannot be empty." });

        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "Benutzer nicht angemeldet." });
        
        var dtos = await _svc.GetByIds(ids);
        var filtered = dtos.Where(b => b.UserId == userId).ToList();
        
        if (filtered.Count == 0)
            return Forbid();
        
        return Ok(filtered);
    }
}
