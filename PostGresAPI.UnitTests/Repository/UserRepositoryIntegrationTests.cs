using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Repository;

// test crude operations with temporary db in ram
public class UserRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        // Arrange
        _context.Users.Add(new User("User1", "user1@test.com"));
        _context.Users.Add(new User("User2", "user2@test.com"));
        await _context.SaveChangesAsync();

        // Act
        var users = await _repository.GetAll();

        // Assert
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var users = await _repository.GetAll();

        // Assert
        Assert.Empty(users);
    }

    [Fact]
    public async Task GetAll_ReturnsSortedById()
    {
        // Arrange
        var user3 = new User("User3", "user3@test.com");
        var user1 = new User("User1", "user1@test.com");
        var user2 = new User("User2", "user2@test.com");

        _context.Users.Add(user3);
        _context.Users.Add(user1);
        _context.Users.Add(user2);
        await _context.SaveChangesAsync();

        // Act
        var users = await _repository.GetAll();

        // Assert
        Assert.Equal(3, users.Count);
        // Verify they are sorted by Id
        for (int i = 0; i < users.Count - 1; i++)
        {
            Assert.True(users[i].Id < users[i + 1].Id);
        }
    }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User("TestUser", "test@test.com");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var userId = user.Id;
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("TestUser", result.UserName);
    }

    [Fact]
    public async Task GetById_NonExistingUser_ReturnsNull()
    {
        // Act
        var result = await _repository.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CreatesUser()
    {
        // Arrange
        var createDto = new CreateUserDto("NewUser", "new@test.com", "1234567890");

        // Act
        var result = await _repository.Add(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("NewUser", result.UserName);
        Assert.Equal("new@test.com", result.Email);

        // Clear and verify
        _context.ChangeTracker.Clear();
        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == result.Id);
        Assert.NotNull(savedUser);
    }

    [Fact]
    public async Task Update_UpdatesUser()
    {
        // Arrange
        var user = new User("OriginalUser", "original@test.com");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var userId = user.Id;
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.Update(userId, "UpdatedUser", "updated@test.com", "0987654321");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("UpdatedUser", result.UserName);
        Assert.Equal("updated@test.com", result.Email);

        // Verify in database
        _context.ChangeTracker.Clear();
        var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        Assert.Equal("UpdatedUser", updatedUser?.UserName);
    }

    [Fact]
    public async Task Update_NonExistingUser_ReturnsNull()
    {
        // Act
        var result = await _repository.Update(999, "UpdatedUser", "updated@test.com", "1234567890");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_RemovesUser()
    {
        // Arrange
        var user = new User("ToDelete", "delete@test.com");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var userId = user.Id;
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.Delete(userId);

        // Assert
        Assert.True(result);

        // Verify it was deleted
        var deletedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Delete_NonExistingUser_ReturnsFalse()
    {
        // Act
        var result = await _repository.Delete(999);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
