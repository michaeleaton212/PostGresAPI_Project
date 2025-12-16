using Moq;
using PostGresAPI.Services;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Services;

// logic tests
public class BedroomServiceTests
{
    private readonly Mock<IBedroomRepository> _mockRepo;
    private readonly BedroomService _service;

    public BedroomServiceTests()
    {
        _mockRepo = new Mock<IBedroomRepository>();
        _service = new BedroomService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllBedrooms()
    {
        // Arrange
        var bedrooms = new List<Bedroom>
        {
            new Bedroom("Bedroom 1", 2),
            new Bedroom("Bedroom 2", 1)
        };

        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bedrooms);

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Bedroom>());

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetById_ExistingBedroom_ReturnsBedroom()
    {
        // Arrange
        var bedroom = new Bedroom("Test Bedroom", 2);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(bedroom);

        // Act
        var result = await _service.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Bedroom", result.Name);
    }

    [Fact]
    public async Task GetById_NonExistingBedroom_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetById(999)).ReturnsAsync((Bedroom?)null);

        // Act
        var result = await _service.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ValidBedroom_ReturnsCreatedBedroom()
    {
        // Arrange
        var createDto = new CreateBedroomDto
        {
            Name = "New Bedroom",
            NumberOfBeds = 2,
            ImagePath = "/images/bedroom.jpg"
        };
        var createdBedroom = new Bedroom("New Bedroom", 2);

        _mockRepo.Setup(r => r.Add(createDto)).ReturnsAsync(createdBedroom);

        // Act
        var result = await _service.Create(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Bedroom", result.Name);
        Assert.Equal(2, result.NumberOfBeds);
    }

    [Fact]
    public async Task Create_WithoutImage_CreatesSuccessfully()
    {
        // Arrange
        var createDto = new CreateBedroomDto
        {
            Name = "Simple Bedroom",
            NumberOfBeds = 1
        };
        var createdBedroom = new Bedroom("Simple Bedroom", 1);

        _mockRepo.Setup(r => r.Add(createDto)).ReturnsAsync(createdBedroom);

        // Act
        var result = await _service.Create(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Simple Bedroom", result.Name);
    }

    [Fact]
    public async Task Update_ExistingBedroom_ReturnsUpdatedBedroom()
    {
        // Arrange
        var updateDto = new UpdateBedroomDto
        {
            Name = "Updated Bedroom",
            NumberOfBeds = 3,
            ImagePath = "/images/updated.jpg"
        };
        var updatedBedroom = new Bedroom("Updated Bedroom", 3);

        _mockRepo.Setup(r => r.Update(1, updateDto)).ReturnsAsync(updatedBedroom);

        // Act
        var result = await _service.Update(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Bedroom", result.Name);
        Assert.Equal(3, result.NumberOfBeds);
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
        _mockRepo.Setup(r => r.Update(999, updateDto)).ReturnsAsync((Bedroom?)null);

        // Act
        var result = await _service.Update(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ExistingBedroom_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(r => r.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _service.Delete(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Delete_NonExistingBedroom_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.Delete(999)).ReturnsAsync(false);

        // Act
        var result = await _service.Delete(999);

        // Assert
        Assert.False(result);
    }
}
