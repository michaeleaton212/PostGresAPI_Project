using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts;
using PostGresAPI.Services;
using PostGresAPI.Models;

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BedroomsController : ControllerBase
{
    private readonly BedroomService _service;

    public BedroomsController(BedroomService service) => _service = service;

    // GET: /api/bedrooms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BedroomDto>>> GetAll()
    {
        var bedrooms = await _service.GetAll();
        var result = bedrooms.Select(b => new BedroomDto(b.Id, b.Name, b.NumberOfBeds));
        return Ok(result);
    }

    // GET: /api/bedrooms/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BedroomDto>> GetById(int id)
    {
        var b = await _service.GetById(id);
        if (b is null) return NotFound();

        var result = new BedroomDto(b.Id, b.Name, b.NumberOfBeds);
        return Ok(result);
    }

    // POST: /api/bedrooms
    [HttpPost]
    public async Task<ActionResult<BedroomDto>> Create([FromBody] CreateBedroomDto dto)
    {
        var created = await _service.Create(dto.Name, dto.NumberOfBeds);
        var result = new BedroomDto(created.Id, created.Name, created.NumberOfBeds);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
    }

    // PUT: /api/bedrooms/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBedroomDto dto)
    {
        var updated = await _service.Update(id, dto.Name, dto.NumberOfBeds);
        if (updated is null) return NotFound();

        var result = new BedroomDto(updated.Id, updated.Name, updated.NumberOfBeds);
        return Ok(result);
    }

    // DELETE: /api/bedrooms/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.Delete(id);
        return success ? NoContent() : NotFound();
    }
}
