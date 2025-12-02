using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts;
using PostGresAPI.Services;

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoomsController : ControllerBase
{
    private readonly IRoomService _service;

    public RoomsController(IRoomService service) => _service = service; // Constructor Injection

    // GET: /api/rooms?type=Meetingroom|Bedroom
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll([FromQuery] string? type = null)
    {
        var rooms = await _service.GetAll(type);
        return Ok(rooms);
    }

    // GET: /api/rooms/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var room = await _service.GetById(id);
        if (room is null) return NotFound();

        return Ok(room);
    }
}
