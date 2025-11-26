namespace PostGresAPI.Auth
{
	public interface ITokenService
	{
		string Create(int bookingId, DateTimeOffset expiresUtc);
		bool TryValidate(string token, out int bookingId);
	}
}
