namespace PostGresAPI.Contracts
{
	public record LoginRequestDto(string BookingNumber, string Name);
	// Response BookingId and signed Token
	public record LoginResponseDto(int BookingId, string Token);
}
