using System;

namespace PostGresAPI.Persistence.Entities
{
    public class OutboxEmail
    {
        public long Id { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public string To { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string HtmlBody { get; set; } = null!;
        public string TextBody { get; set; } = null!;

        public DateTime? SentUtc { get; set; }

        public int TryCount { get; set; } = 0;

        public string? LastError { get; set; }
    }
}
