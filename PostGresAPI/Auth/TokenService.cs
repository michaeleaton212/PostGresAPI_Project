using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PostGresAPI.Auth
{
    public class TokenService : ITokenService
    {
        private readonly byte[] _key;

        public TokenService(IConfiguration cfg)
        {
            var secret = cfg["Auth:LoginTokenSecret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("Auth:LoginTokenSecret is not configured.");
            _key = Encoding.UTF8.GetBytes(secret);
        }

        public string Create(int bookingId, DateTimeOffset expiresUtc)
        {
            var payload = $"{bookingId}|{expiresUtc.ToUnixTimeSeconds()}";
            using var hmac = new HMACSHA256(_key);
            var sig = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}|{sig}"));
        }

        public bool TryValidate(string token, out int bookingId)
        {
            bookingId = 0;
            string decoded;
            try { decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token)); }
            catch { return false; }

            var parts = decoded.Split('|');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out bookingId)) return false;
            if (!long.TryParse(parts[1], out var expUnix)) return false;

            var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix);
            if (DateTimeOffset.UtcNow > exp) return false;

            var payload = $"{parts[0]}|{parts[1]}";
            using var hmac = new HMACSHA256(_key);
            var expected = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            return expected == parts[2];
        }
    }
}
