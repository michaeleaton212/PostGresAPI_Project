using PostGresAPI.Models;

namespace PostGresAPI.Interfaces.IRepository;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(int id);
    Task<IEnumerable<Review>> GetByUserIdAsync(int userId);
    Task<Review> CreateAsync(Review review);
    Task<Review> UpdateAsync(Review review);
    Task<bool> DeleteAsync(int id);
}
