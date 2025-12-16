using Microsoft.AspNetCore.Mvc;
using Moq;
using PostGresAPI.Controllers;
using PostGresAPI.Services;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Controllers;

// tests HTTP-Result-Typ, Data in Result, and mapping of the data (Output test)
// defines service output in mock and verifies controller output
public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _controller = new UsersController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto(1, "User1", "user1@test.com"),
            new UserDto(2, "User2", "user2@test.com")
        };

        _mockService.Setup(s => s.GetAll()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count());
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAll()).ReturnsAsync(new List<UserDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
        Assert.Empty(returnedUsers);
    }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new UserDto(1, "TestUser", "test@test.com");
        _mockService.Setup(s => s.GetById(1)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(1, returnedUser.Id);
        Assert.Equal("TestUser", returnedUser.UserName);
    }

    [Fact]
    public async Task GetById_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetById(999)).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task GetById_VariousIds_ReturnsCorrectUser(int userId)
    {
        // Arrange
        var user = new UserDto(userId, $"User{userId}", $"user{userId}@test.com");
        _mockService.Setup(s => s.GetById(userId)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userId, returnedUser.Id);
    }

    [Fact]
    public async Task Create_ValidUser_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateUserDto("NewUser", "new@test.com", "1234567890");
        var createdUser = new UserDto(1, "NewUser", "new@test.com");

        _mockService.Setup(s => s.Create(createDto)).ReturnsAsync(createdUser);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedUser = Assert.IsType<UserDto>(createdResult.Value);
        Assert.Equal(1, returnedUser.Id);
        Assert.Equal("NewUser", returnedUser.UserName);
    }

    [Fact]
    public async Task Update_ExistingUser_ReturnsUpdatedUser()
    {
        // Arrange
        var updateDto = new UpdateUserDto("UpdatedUser", "updated@test.com", "0987654321");
        var updatedUser = new UserDto(1, "UpdatedUser", "updated@test.com");

        _mockService.Setup(s => s.Update(1, updateDto)).ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal("UpdatedUser", returnedUser.UserName);
    }

    [Fact]
    public async Task Update_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateUserDto("UpdatedUser", "updated@test.com", "0987654321");
        _mockService.Setup(s => s.Update(999, updateDto)).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ExistingUser_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.Delete(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
