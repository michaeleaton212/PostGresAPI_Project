namespace PostGresAPI.Models;

public class Review
{
    private Review() { } // EF

    public Review(int userId, string title, string content, int rating)
    {
        UserId = userId;
        Title = title;
        Content = content;
        Rating = rating;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Title { get; private set; } = "";
    public string Content { get; private set; } = "";
    public int Rating { get; private set; } // 1-5 stars
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public User? User { get; private set; }

    public void Update(string title, string content, int rating)
    {
        Title = title;
        Content = content;
        Rating = rating;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
