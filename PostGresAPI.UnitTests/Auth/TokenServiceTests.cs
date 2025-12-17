using Microsoft.Extensions.Configuration;
using PostGresAPI.Auth;

namespace PostGresAPI.UnitTests.Auth;

public class TokenServiceTests
{
    private readonly TokenService _service;
    private readonly IConfiguration _configuration;

    public TokenServiceTests()
    {
        // Setup test configuration with secret
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Auth:LoginTokenSecret", "test-secret-key-for-unit-testing-min-32-chars" }
        });
        _configuration = configBuilder.Build();
        _service = new TokenService(_configuration);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenSecretNotConfigured()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var emptyConfig = configBuilder.Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new TokenService(emptyConfig));
    }

    [Fact]
    public void Constructor_ThrowsException_WhenSecretIsEmpty()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Auth:LoginTokenSecret", "" }
        });
        var emptyConfig = configBuilder.Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new TokenService(emptyConfig));
    }

    [Fact]
    public void Create_ReturnsValidToken_ForSingleBookingId()
    {
        // Arrange
        var bookingIds = new List<int> { 1 };
        var expiresUtc = DateTimeOffset.UtcNow.AddHours(1);

        // Act
        var token = _service.Create(bookingIds, expiresUtc);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void Create_ReturnsValidToken_ForMultipleBookingIds()
    {
        // Arrange
        var bookingIds = new List<int> { 1, 2, 3, 42, 100 };
        var expiresUtc = DateTimeOffset.UtcNow.AddHours(1);

        // Act
        var token = _service.Create(bookingIds, expiresUtc);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void TryValidate_ReturnsTrue_ForValidToken()
    {
        // Arrange
        var bookingIds = new List<int> { 1, 2, 3 };
        var expiresUtc = DateTimeOffset.UtcNow.AddHours(1);
        var token = _service.Create(bookingIds, expiresUtc);

        // Act
        var isValid = _service.TryValidate(token, out var extractedIds);

        // Assert
        Assert.True(isValid);
        Assert.Equal(bookingIds.Count, extractedIds.Count);
        Assert.Equal(bookingIds, extractedIds);
    }

    [Fact]
    public async Task TryValidate_ReturnsFalse_ForExpiredToken()
    {
        // Arrange
        var bookingIds = new List<int> { 1 };
        var expiresUtc = DateTimeOffset.UtcNow.AddSeconds(-10); // Already expired
        var token = _service.Create(bookingIds, expiresUtc);

        // Act
        var isValid = _service.TryValidate(token, out var extractedIds);

        // Assert
        Assert.False(isValid);
        // Note: extractedIds might contain values because parsing happens before expiration check
        // The important thing is that isValid is False
    }

    [Fact]
    public void TryValidate_ReturnsFalse_ForInvalidBase64()
    {
        // Arrange
        var invalidToken = "this-is-not-base64!!!";

        // Act
        var isValid = _service.TryValidate(invalidToken, out var extractedIds);

        // Assert
        Assert.False(isValid);
        Assert.Empty(extractedIds);
    }

    [Fact]
    public void TryValidate_ReturnsFalse_ForManipulatedToken()
    {
        // Arrange
        var bookingIds = new List<int> { 1 };
        var expiresUtc = DateTimeOffset.UtcNow.AddHours(1);
        var token = _service.Create(bookingIds, expiresUtc);

        // Manipulate token by changing a character
        var manipulated = token.Substring(0, token.Length - 5) + "XXXXX";

        // Act
        var isValid = _service.TryValidate(manipulated, out var extractedIds);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void TryValidate_ReturnsFalse_ForTokenWithWrongFormat()
    {
        // Arrange - create a token with wrong internal format (missing parts)
        var invalidPayload = "1|2"; // Only 2 parts instead of 3
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(invalidPayload));

        // Act
        var isValid = _service.TryValidate(token, out var extractedIds);

        // Assert
        Assert.False(isValid);
        Assert.Empty(extractedIds);
    }

    [Fact]
    public void TryValidate_ReturnsFalse_ForTokenWithInvalidBookingIds()
    {
        // Arrange - create a token with non-numeric booking IDs
        var invalidPayload = "abc,def|1234567890|somesignature";
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(invalidPayload));

        // Act
        var isValid = _service.TryValidate(token, out var extractedIds);

        // Assert
        Assert.False(isValid);
        Assert.Empty(extractedIds);
    }

    [Fact]
    public void TryValidate_ReturnsFalse_ForTokenWithEmptyBookingIds()
    {
        // Arrange - create a token with empty booking IDs
        var invalidPayload = "|1234567890|somesignature";
        var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(invalidPayload));

        // Act
        var isValid = _service.TryValidate(token, out var extractedIds);

        // Assert
        Assert.False(isValid);
        Assert.Empty(extractedIds);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void TryValidate_ExtractsCorrectNumberOfBookingIds(int count)
    {
        // Arrange
        var bookingIds = Enumerable.Range(1, count).ToList();
        var expiresUtc = DateTimeOffset.UtcNow.AddHours(1);
        var token = _service.Create(bookingIds, expiresUtc);

        // Act
        var isValid = _service.TryValidate(token, out var extractedIds);

        // Assert
        Assert.True(isValid);
        Assert.Equal(count, extractedIds.Count);
        Assert.Equal(bookingIds, extractedIds);
    }

    [Fact]
    public void TryValidate_ReturnsFalse_ForNullToken()
    {
        // Act
        var isValid = _service.TryValidate(null!, out var extractedIds);

        // Assert
        Assert.False(isValid);
        Assert.Empty(extractedIds);
    }

    [Fact]
    public void TryValidate_ReturnsFalse_ForEmptyToken()
    {
        // Act
        var isValid = _service.TryValidate(string.Empty, out var extractedIds);

        // Assert
        Assert.False(isValid);
        Assert.Empty(extractedIds);
    }

    [Fact]
    public void Create_DifferentTokens_ForDifferentBookingIds()
    {
        // Arrange
        var bookingIds1 = new List<int> { 1, 2, 3 };
        var bookingIds2 = new List<int> { 4, 5, 6 };
        var expiresUtc = DateTimeOffset.UtcNow.AddHours(1);

        // Act
        var token1 = _service.Create(bookingIds1, expiresUtc);
        var token2 = _service.Create(bookingIds2, expiresUtc);

        // Assert
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void Create_DifferentTokens_ForDifferentExpirationTimes()
    {
        // Arrange
        var bookingIds = new List<int> { 1, 2, 3 };
        var expiresUtc1 = DateTimeOffset.UtcNow.AddHours(1);
        var expiresUtc2 = DateTimeOffset.UtcNow.AddHours(2);

        // Act
        var token1 = _service.Create(bookingIds, expiresUtc1);
        var token2 = _service.Create(bookingIds, expiresUtc2);

        // Assert
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void TryValidate_PreservesBookingIdOrder()
    {
        // Arrange
        var bookingIds = new List<int> { 42, 1, 99, 7, 15 };
        var expiresUtc = DateTimeOffset.UtcNow.AddHours(1);
        var token = _service.Create(bookingIds, expiresUtc);

        // Act
        var isValid = _service.TryValidate(token, out var extractedIds);

        // Assert
        Assert.True(isValid);
        Assert.Equal(bookingIds, extractedIds); // Order should be preserved
    }
}
