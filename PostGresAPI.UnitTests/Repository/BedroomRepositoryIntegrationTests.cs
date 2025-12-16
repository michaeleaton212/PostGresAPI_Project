using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Repository;

// test crude operations with temporary db in ram
public class BedroomRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BedroomRepository _repository;

    public BedroomRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BedroomRepository(_context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllBedrooms()
    {
        // Arrange
        _context.Bedrooms.Add(new Bedroom("Bedroom 1", 2));
        _context.Bedrooms.Add(new Bedroom("Bedroom 2", 1));
        await _context.SaveChangesAsync();

        // Act
        var bedrooms = await _repository.GetAll();

        // Assert
        Assert.Equal(2, bedrooms.Count);
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var bedrooms = await _repository.GetAll();

        // Assert
        Assert.Empty(bedrooms);
    }

    [Fact]
    public async Task GetAll_ReturnsSortedById()
    {
        // Arrange
        var bedroom3 = new Bedroom("Bedroom 3", 3);
        var bedroom1 = new Bedroom("Bedroom 1", 1);
        var bedroom2 = new Bedroom("Bedroom 2", 2);

        _context.Bedrooms.Add(bedroom3);
        _context.Bedrooms.Add(bedroom1);
        _context.Bedrooms.Add(bedroom2);
        await _context.SaveChangesAsync();

        // Act
        var bedrooms = await _repository.GetAll();

        // Assert
        Assert.Equal(3, bedrooms.Count);
        // Verify they are sorted by Id
        for (int i = 0; i < bedrooms.Count - 1; i++)
        {
            Assert.True(bedrooms[i].Id < bedrooms[i + 1].Id);
        }
    }

    [Fact]
    public async Task GetById_ExistingBedroom_ReturnsBedroom()
    {
        // Arrange
        var bedroom = new Bedroom("Test Bedroom", 2);
        _context.Bedrooms.Add(bedroom);
        await _context.SaveChangesAsync();
        var bedroomId = bedroom.Id;
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetById(bedroomId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bedroomId, result.Id);
        Assert.Equal("Test Bedroom", result.Name);
    }

    [Fact]
    public async Task GetById_NonExistingBedroom_ReturnsNull()
    {
        // Act
        var result = await _repository.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CreatesBedroom()
    {
        // Arrange
        var createDto = new CreateBedroomDto
        {
            Name = "New Bedroom",
            NumberOfBeds = 2,
            ImagePath = "/images/bedroom.jpg"
        };

        // Act
        var result = await _repository.Add(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Bedroom", result.Name);
        Assert.Equal(2, result.NumberOfBeds);

        // Clear and verify
        _context.ChangeTracker.Clear();
        var savedBedroom = await _context.Bedrooms.FirstOrDefaultAsync(b => b.Id == result.Id);
        Assert.NotNull(savedBedroom);
    }

    [Fact]
    public async Task Update_UpdatesBedroom()
    {
        // Arrange
        var bedroom = new Bedroom("Original Bedroom", 2);
        _context.Bedrooms.Add(bedroom);
        await _context.SaveChangesAsync();
        var bedroomId = bedroom.Id;
        _context.ChangeTracker.Clear();

        var updateDto = new UpdateBedroomDto
        {
            Name = "Updated Bedroom",
            NumberOfBeds = 3,
            ImagePath = "/images/updated.jpg"
        };

        // Act
        var result = await _repository.Update(bedroomId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Bedroom", result.Name);
        Assert.Equal(3, result.NumberOfBeds);

        // Verify in database
        _context.ChangeTracker.Clear();
        var updatedBedroom = await _context.Bedrooms.FirstOrDefaultAsync(b => b.Id == bedroomId);
        Assert.Equal("Updated Bedroom", updatedBedroom?.Name);
    }

    [Fact]
    public async Task Update_NonExistingBedroom_ReturnsNull()
    {
        // Arrange
        var updateDto = new UpdateBedroomDto
        {
            Name = "Updated Bedroom",
            NumberOfBeds = 2
        };

        // Act
        var result = await _repository.Update(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_ExistingBedroom_ReturnsTrue()
    {
        // Arrange
        var bedroom = new Bedroom("Test Bedroom", 2);
        _context.Bedrooms.Add(bedroom);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(bedroom.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_NonExistingBedroom_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Delete_RemovesBedroom()
    {
        // Arrange
        var bedroom = new Bedroom("To Delete", 2);
        _context.Bedrooms.Add(bedroom);
        await _context.SaveChangesAsync();
        var bedroomId = bedroom.Id;
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.Delete(bedroomId);

        // Assert
        Assert.True(result);

        // Verify it was deleted
        var deletedBedroom = await _context.Bedrooms.FirstOrDefaultAsync(b => b.Id == bedroomId);
        Assert.Null(deletedBedroom);
    }

    [Fact]
    public async Task Delete_NonExistingBedroom_ReturnsFalse()
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
