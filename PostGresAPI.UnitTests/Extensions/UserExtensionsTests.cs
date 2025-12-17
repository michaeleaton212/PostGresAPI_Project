using PostGresAPI.Extensions;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Extensions;

public class UserExtensionsTests
{
    [Fact]
    public void ToDto_ValidUser_ReturnsMappedDto()
    {
        // Arrange
        var user = new User("TestUser", "test@example.com");

        // Act
        var result = user.ToDto();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestUser", result.UserName);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public void ToEntity_ValidCreateDto_ReturnsUserEntity()
    {
        // Arrange
        var dto = new CreateUserDto("NewUser", "new@example.com", "1234567890");

        // Act
        var result = dto.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewUser", result.UserName);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public void UpdateEntity_ValidUpdateDto_UpdatesUserProperties()
    {
        // Arrange
        var user = new User("OldName", "old@example.com");
        var updateDto = new UpdateUserDto("UpdatedName", "updated@example.com", "0987654321");

        // Act
        updateDto.UpdateEntity(user);

        // Assert
        Assert.Equal("UpdatedName", user.UserName);
        Assert.Equal("updated@example.com", user.Email);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("SingleName", "single@test.com")]
    [InlineData("Very Long Username With Spaces", "very.long.email@subdomain.example.com")]
    public void ToDto_VariousUserNames_MapsCorrectly(string userName, string email)
    {
        // Arrange
        var user = new User(userName, email);

        // Act
        var result = user.ToDto();

        // Assert
        Assert.Equal(userName, result.UserName);
        Assert.Equal(email, result.Email);
    }
}
