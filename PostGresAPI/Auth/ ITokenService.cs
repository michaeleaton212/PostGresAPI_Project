namespace PostGresAPI.Auth
{
	public interface ITokenService
	{
		string Create(List<int> bookingIds, DateTimeOffset expiresUtc);
		bool TryValidate(string token, out List<int> bookingIds);
	}
}
