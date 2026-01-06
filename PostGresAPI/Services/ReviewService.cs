using PostGresAPI.Contracts;
using PostGresAPI.Extensions;
using PostGresAPI.Interfaces.IRepository;
using PostGresAPI.Interfaces.IServices;

namespace PostGresAPI.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        return reviews.Select(r => r.ToDto());
    }

    public async Task<ReviewDto?> GetReviewByIdAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        return review?.ToDto();
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId)
    {
        var reviews = await _reviewRepository.GetByUserIdAsync(userId);
        return reviews.Select(r => r.ToDto());
    }

    public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int userId)
    {
        var review = dto.ToEntity(userId);
        var created = await _reviewRepository.CreateAsync(review);
        return created.ToDto();
    }

    public async Task<ReviewDto?> UpdateReviewAsync(int id, UpdateReviewDto dto, int userId)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null || review.UserId != userId)
            return null;

        review.ApplyUpdate(dto);
        var updated = await _reviewRepository.UpdateAsync(review);
        return updated.ToDto();
    }

    public async Task<bool> DeleteReviewAsync(int id, int userId)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null || review.UserId != userId)
            return false;

        return await _reviewRepository.DeleteAsync(id);
    }
}
