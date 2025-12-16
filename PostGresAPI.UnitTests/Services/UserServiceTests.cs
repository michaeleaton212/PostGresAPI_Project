using Moq;
using PostGresAPI.Services;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Services;

// logic tests
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _service = new UserService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User("User1", "user1@test.com"),
            new User("User2", "user2@test.com")
        };

        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(users);

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User("TestUser", "test@test.com");
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(user);

        // Act
        var result = await _service.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestUser", result.UserName);
    }

    [Fact]
    public async Task GetById_NonExistingUser_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetById(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ValidUser_ReturnsCreatedUser()
    {
        // Arrange
        var createDto = new CreateUserDto("NewUser", "new@test.com", "1234567890");
        var createdUser = new User("NewUser", "new@test.com");

        _mockRepo.Setup(r => r.Add(createDto)).ReturnsAsync(createdUser);

        // Act
        var result = await _service.Create(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewUser", result.UserName);
        Assert.Equal("new@test.com", result.Email);
    }

    [Fact]
    public async Task Update_ExistingUser_ReturnsUpdatedUser()
    {
        // Arrange
        var updateDto = new UpdateUserDto("UpdatedUser", "updated@test.com", "0987654321");
        var updatedUser = new User("UpdatedUser", "updated@test.com");

        _mockRepo.Setup(r => r.Update(1, updateDto.UserName, updateDto.Email, updateDto.Phone))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _service.Update(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UpdatedUser", result.UserName);
        Assert.Equal("updated@test.com", result.Email);
    }

    [Fact]
    public async Task Update_NonExistingUser_ReturnsNull()
    {
        // Arrange
        var updateDto = new UpdateUserDto("UpdatedUser", "updated@test.com", "0987654321");
        _mockRepo.Setup(r => r.Update(999, updateDto.UserName, updateDto.Email, updateDto.Phone))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.Update(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ExistingUser_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(r => r.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _service.Delete(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Delete_NonExistingUser_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.Delete(999)).ReturnsAsync(false);

        // Act
        var result = await _service.Delete(999);

        // Assert
        Assert.False(result);
    }
}
