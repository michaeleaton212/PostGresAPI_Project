namespace PostGresAPI.Contracts
{
	public record LoginRequestDto(string BookingNumber, string Name);
	// Response with all BookingIds for this user and signed Token
	public record LoginResponseDto(List<int> BookingIds, string Token);
}
