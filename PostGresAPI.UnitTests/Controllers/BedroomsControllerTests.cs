using Microsoft.AspNetCore.Mvc;
using Moq;
using PostGresAPI.Controllers;
using PostGresAPI.Services;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Controllers;

// tests HTTP-Result-Typ, Data in Result, and mapping of the data (Output test)
// defines service output in mock and verifies controller output
public class BedroomsControllerTests
{
    private readonly Mock<IBedroomService> _mockService;
    private readonly BedroomsController _controller;

    public BedroomsControllerTests()
    {
        _mockService = new Mock<IBedroomService>();
        _controller = new BedroomsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllBedrooms()
    {
        // Arrange
        var bedrooms = new List<BedroomDto>
        {
            new BedroomDto { Id = 1, Name = "Bedroom 1", NumberOfBeds = 2, PricePerNight = 100.00m },
            new BedroomDto { Id = 2, Name = "Bedroom 2", NumberOfBeds = 1, PricePerNight = 75.50m }
        };

        _mockService.Setup(s => s.GetAll()).ReturnsAsync(bedrooms);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBedrooms = Assert.IsAssignableFrom<IEnumerable<BedroomDto>>(okResult.Value);
        Assert.Equal(2, returnedBedrooms.Count());
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAll()).ReturnsAsync(new List<BedroomDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBedrooms = Assert.IsAssignableFrom<IEnumerable<BedroomDto>>(okResult.Value);
        Assert.Empty(returnedBedrooms);
    }

    [Fact]
    public async Task GetById_ExistingBedroom_ReturnsBedroom()
    {
        // Arrange
        var bedroom = new BedroomDto { Id = 1, Name = "Test Bedroom", NumberOfBeds = 2, PricePerNight = 120.00m };
        _mockService.Setup(s => s.GetById(1)).ReturnsAsync(bedroom);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBedroom = Assert.IsType<BedroomDto>(okResult.Value);
        Assert.Equal(1, returnedBedroom.Id);
        Assert.Equal("Test Bedroom", returnedBedroom.Name);
        Assert.Equal(120.00m, returnedBedroom.PricePerNight);
    }

    [Fact]
    public async Task GetById_NonExistingBedroom_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetById(999)).ReturnsAsync((BedroomDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task GetById_VariousIds_ReturnsCorrectBedroom(int bedroomId)
    {
        // Arrange
        var bedroom = new BedroomDto { Id = bedroomId, Name = $"Bedroom {bedroomId}", NumberOfBeds = 2, PricePerNight = 100.00m + bedroomId };
        _mockService.Setup(s => s.GetById(bedroomId)).ReturnsAsync(bedroom);

        // Act
        var result = await _controller.GetById(bedroomId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBedroom = Assert.IsType<BedroomDto>(okResult.Value);
        Assert.Equal(bedroomId, returnedBedroom.Id);
    }

    [Fact]
    public async Task Create_ValidBedroom_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateBedroomDto
        {
            Name = "New Bedroom",
            NumberOfBeds = 2,
            ImagePath = "/images/bedroom.jpg",
            PricePerNight = 150.00m
        };
        var createdBedroom = new BedroomDto { Id = 1, Name = "New Bedroom", NumberOfBeds = 2, PricePerNight = 150.00m };

        _mockService.Setup(s => s.Create(createDto)).ReturnsAsync(createdBedroom);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBedroom = Assert.IsType<BedroomDto>(createdResult.Value);
        Assert.Equal(1, returnedBedroom.Id);
        Assert.Equal("New Bedroom", returnedBedroom.Name);
        Assert.Equal(150.00m, returnedBedroom.PricePerNight);
    }

    [Fact]
    public async Task Create_WithoutImage_CreatesSuccessfully()
    {
        // Arrange
        var createDto = new CreateBedroomDto
        {
            Name = "Simple Bedroom",
            NumberOfBeds = 1,
            PricePerNight = 80.00m
        };
        var createdBedroom = new BedroomDto { Id = 1, Name = "Simple Bedroom", NumberOfBeds = 1, PricePerNight = 80.00m };

        _mockService.Setup(s => s.Create(createDto)).ReturnsAsync(createdBedroom);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBedroom = Assert.IsType<BedroomDto>(createdResult.Value);
        Assert.Equal("Simple Bedroom", returnedBedroom.Name);
        Assert.Equal(80.00m, returnedBedroom.PricePerNight);
    }

    [Fact]
    public async Task Update_ExistingBedroom_ReturnsUpdatedBedroom()
    {
        // Arrange
        var updateDto = new UpdateBedroomDto
        {
            Name = "Updated Bedroom",
            NumberOfBeds = 3,
            ImagePath = "/images/updated.jpg",
            PricePerNight = 200.00m
        };
        var updatedBedroom = new BedroomDto { Id = 1, Name = "Updated Bedroom", NumberOfBeds = 3, PricePerNight = 200.00m };

        _mockService.Setup(s => s.Update(1, updateDto)).ReturnsAsync(updatedBedroom);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBedroom = Assert.IsType<BedroomDto>(okResult.Value);
        Assert.Equal("Updated Bedroom", returnedBedroom.Name);
        Assert.Equal(3, returnedBedroom.NumberOfBeds);
        Assert.Equal(200.00m, returnedBedroom.PricePerNight);
    }

    [Fact]
    public async Task Update_NonExistingBedroom_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateBedroomDto
        {
            Name = "Updated Bedroom",
            NumberOfBeds = 2,
            PricePerNight = 100.00m
        };
        _mockService.Setup(s => s.Update(999, updateDto)).ReturnsAsync((BedroomDto?)null);

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ExistingBedroom_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingBedroom_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.Delete(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
