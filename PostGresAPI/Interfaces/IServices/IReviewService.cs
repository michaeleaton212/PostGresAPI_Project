using PostGresAPI.Contracts;

namespace PostGresAPI.Interfaces.IServices;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
    Task<ReviewDto?> GetReviewByIdAsync(int id);
    Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(int userId);
    Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int userId);
    Task<ReviewDto?> UpdateReviewAsync(int id, UpdateReviewDto dto, int userId);
    Task<bool> DeleteReviewAsync(int id, int userId);
}
