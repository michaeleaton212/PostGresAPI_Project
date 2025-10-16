using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts;
using PostGresAPI.Models;
using PostGresAPI.Services;

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RoomsController : ControllerBase
{
    private readonly RoomService _service;

    public RoomsController(RoomService service) => _service = service; // Constructor Injection§

    // GET: /api/rooms?type=Meetingroom|Bedroom
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll([FromQuery] string? type = null)
    {
        var rooms = await _service.GetAll();

        // optionaler Filter nach Typ
        if (!string.IsNullOrWhiteSpace(type))
        {
            rooms = rooms.Where(r =>
                (type.Equals("Meetingroom", StringComparison.OrdinalIgnoreCase) && r is Meetingroom) ||
                (type.Equals("Bedroom", StringComparison.OrdinalIgnoreCase) && r is Bedroom)
            ).ToList();
        }

        // Map to dto with type info
        var result = rooms.Select(r =>
        {
            var roomType = r switch
            {
                Meetingroom => "Meetingroom",
                Bedroom => "Bedroom",
                _ => "Room"
            };
            return new RoomDto(r.Id, r.Name, roomType);
        });

        return Ok(result);
    }

    // GET: /api/rooms/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var room = await _service.GetById(id);
        if (room is null) return NotFound();

        var roomType = room switch
        {
            Meetingroom => "Meetingroom",
            Bedroom => "Bedroom",
            _ => "Room"
        };

        var dto = new RoomDto(room.Id, room.Name, roomType);
        return Ok(dto);
    }
}
