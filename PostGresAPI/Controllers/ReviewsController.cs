using Microsoft.AspNetCore.Mvc;
using PostGresAPI.Contracts;
using PostGresAPI.Interfaces.IServices;

namespace PostGresAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // Helper method to get user ID from session
    private int? GetUserIdFromSession()
    {
        var userIdStr = Request.Headers["X-User-Id"].FirstOrDefault();
        if (int.TryParse(userIdStr, out var userId))
            return userId;
        return null;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAll()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetById(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        if (review == null)
            return NotFound();
        
        return Ok(review);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByUserId(int userId)
    {
        var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto dto)
    {
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "You must be logged in to create a review." });

        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5");

        var review = await _reviewService.CreateReviewAsync(dto, userId.Value);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewDto>> Update(int id, [FromBody] UpdateReviewDto dto)
    {
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "You must be logged in to update a review." });

        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest("Rating must be between 1 and 5");

        var review = await _reviewService.UpdateReviewAsync(id, dto, userId.Value);
        
        if (review == null)
            return NotFound(new { error = "Review not found or you don't have permission to update it." });
        
        return Ok(review);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = GetUserIdFromSession();
        if (userId == null)
            return Unauthorized(new { error = "You must be logged in to delete a review." });

        var deleted = await _reviewService.DeleteReviewAsync(id, userId.Value);
        
        if (!deleted)
            return NotFound(new { error = "Review not found or you don't have permission to delete it." });
        
        return NoContent();
    }
}
