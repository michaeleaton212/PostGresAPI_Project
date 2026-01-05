using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PostGresAPI.Auth
{
    public class TokenService : ITokenService
    {
        private readonly byte[] _key;

        public TokenService(IConfiguration cfg) // get secret from config
        {
            var secret = cfg["Auth:LoginTokenSecret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("Auth:LoginTokenSecret is not configured.");
            _key = Encoding.UTF8.GetBytes(secret);
        }

        public string Create(List<int> bookingIds, DateTimeOffset expiresUtc)
        {
            var idsString = string.Join(",", bookingIds);// out of array string
            var payload = $"{idsString}|{expiresUtc.ToUnixTimeSeconds()}"; // separates fields
            using var hmac = new HMACSHA256(_key);// HMAC-SHA256-Objekt
            var sig = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))); // calculate the token signature
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{payload}|{sig}"));
        }

        public bool TryValidate(string token, out List<int> bookingIds) // validate token and extract booking IDs
        {
            bookingIds = new List<int>(); // new list for booking IDs
            string decoded;
            try { decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token)); } // decodes token from Base64 and converts to utf 8 string 
            catch { return false; }

            var parts = decoded.Split('|'); //splits de string at every '|'
            if (parts.Length != 3) return false; // must have 3 parts

            // Parse booking IDs
            var idStrings = parts[0].Split(',', StringSplitOptions.RemoveEmptyEntries); // splits the first part in to single ids (",")
            foreach (var idStr in idStrings) // tries to parse each id string to int and adds to bookingIds list
            {
                if (!int.TryParse(idStr, out var id)) return false;
                bookingIds.Add(id);
            }
            
            if (bookingIds.Count == 0) return false;
            if (!long.TryParse(parts[1], out var expUnix)) return false;

            var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix); // parses expiration time to UTC date time
            if (DateTimeOffset.UtcNow > exp) return false;//  check if token is expired

            var payload = $"{parts[0]}|{parts[1]}";
            using var hmac = new HMACSHA256(_key);
            var expected = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
            return expected == parts[2];
        }
    }
}
