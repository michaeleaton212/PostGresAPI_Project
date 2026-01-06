using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.Extensions;

public static class ReviewMappingExtensions
{
    public static ReviewDto ToDto(this Review review)
    {
        return new ReviewDto(
            review.Id,
            review.UserId,
            review.User?.UserName ?? "",
            review.Title,
            review.Content,
            review.Rating,
            review.CreatedAt,
            review.UpdatedAt
        );
    }

    public static Review ToEntity(this CreateReviewDto dto, int userId)
    {
        return new Review(userId, dto.Title, dto.Content, dto.Rating);
    }

    public static void ApplyUpdate(this Review review, UpdateReviewDto dto)
    {
        review.Update(dto.Title, dto.Content, dto.Rating);
    }
}
