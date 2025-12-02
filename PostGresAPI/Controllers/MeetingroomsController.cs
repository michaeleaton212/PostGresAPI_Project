using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts;
using PostGresAPI.Services;
using PostGresAPI.Models;

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MeetingroomsController : ControllerBase
{
    private readonly IMeetingroomService _service;
    public MeetingroomsController(IMeetingroomService service) => _service = service; // Constructor Injection

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeetingroomDto>>> GetAll()
    {
        var meetingrooms = await _service.GetAll();
        var result = meetingrooms.Select(m => new MeetingroomDto(m.Id, m.Name, m.NumberOfChairs, m.ImagePath));
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MeetingroomDto>> GetById(int id)
    {
        var m = await _service.GetById(id);
        if (m is null) return NotFound();

        var result = new MeetingroomDto(m.Id, m.Name, m.NumberOfChairs, m.ImagePath);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MeetingroomDto>> Create([FromBody] CreateMeetingroomDto dto)
    {
        var created = await _service.Create(dto); // pass DTO
        var result = new MeetingroomDto(created.Id, created.Name, created.NumberOfChairs, created.ImagePath);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MeetingroomDto>> Update(int id, [FromBody] UpdateMeetingroomDto dto)
    {
        var updated = await _service.Update(id, dto); // pass DTO
        if (updated is null) return NotFound();

        var result = new MeetingroomDto(updated.Id, updated.Name, updated.NumberOfChairs, updated.ImagePath);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.Delete(id);
        return deleted ? NoContent() : NotFound();
    }
}
