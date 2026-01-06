namespace PostGresAPI.Contracts;

public record ReviewDto(int Id, int UserId, string UserName, string Title, string Content, int Rating, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);
public record CreateReviewDto(string Title, string Content, int Rating);
public record UpdateReviewDto(string Title, string Content, int Rating);
