using Microsoft.AspNetCore.Mvc;
using Moq;
using PostGresAPI.Controllers;
using PostGresAPI.Services;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Controllers;

public class RoomsControllerTests
{
    private readonly Mock<IRoomService> _mockService;
    private readonly RoomsController _controller;

    public RoomsControllerTests()
    {
        _mockService = new Mock<IRoomService>();
        _controller = new RoomsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_NoFilter_ReturnsAllRooms()
    {
        // Arrange
        var rooms = new List<RoomDto>
        {
            new RoomDto { Id = 1, Name = "Room 1", Type = "Bedroom" },
            new RoomDto { Id = 2, Name = "Room 2", Type = "Meetingroom", NumberOfChairs = 10 }
        };

        _mockService.Setup(s => s.GetAll(null)).ReturnsAsync(rooms);

        // Act
        var result = await _controller.GetAll(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomDto>>(okResult.Value);
        Assert.Equal(2, returnedRooms.Count());
    }

    [Fact]
    public async Task GetAll_FilterByBedroom_ReturnsOnlyBedrooms()
    {
        // Arrange
        var bedrooms = new List<RoomDto>
        {
            new RoomDto { Id = 1, Name = "Bedroom 1", Type = "Bedroom" }
        };

        _mockService.Setup(s => s.GetAll("Bedroom")).ReturnsAsync(bedrooms);

        // Act
        var result = await _controller.GetAll("Bedroom");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomDto>>(okResult.Value);
        Assert.Single(returnedRooms);
        Assert.All(returnedRooms, r => Assert.Equal("Bedroom", r.Type));
    }

    [Fact]
    public async Task GetAll_FilterByMeetingroom_ReturnsOnlyMeetingrooms()
    {
        // Arrange
        var meetingrooms = new List<RoomDto>
        {
            new RoomDto { Id = 2, Name = "Meeting 1", Type = "Meetingroom", NumberOfChairs = 10 }
        };

        _mockService.Setup(s => s.GetAll("Meetingroom")).ReturnsAsync(meetingrooms);

        // Act
        var result = await _controller.GetAll("Meetingroom");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomDto>>(okResult.Value);
        Assert.Single(returnedRooms);
        Assert.All(returnedRooms, r => Assert.Equal("Meetingroom", r.Type));
    }

    [Fact]
    public async Task GetAll_UnknownFilter_ReturnsEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAll("Unknown")).ReturnsAsync(new List<RoomDto>());

        // Act
        var result = await _controller.GetAll("Unknown");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomDto>>(okResult.Value);
        Assert.Empty(returnedRooms);
    }

    [Fact]
    public async Task GetById_ExistingRoom_ReturnsRoom()
    {
        // Arrange
        var room = new RoomDto { Id = 1, Name = "Test Room", Type = "Bedroom" };
        _mockService.Setup(s => s.GetById(1)).ReturnsAsync(room);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRoom = Assert.IsType<RoomDto>(okResult.Value);
        Assert.Equal(1, returnedRoom.Id);
        Assert.Equal("Test Room", returnedRoom.Name);
    }

    [Fact]
    public async Task GetById_NonExistingRoom_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetById(999)).ReturnsAsync((RoomDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task GetById_VariousIds_ReturnsCorrectRoom(int roomId)
    {
        // Arrange
        var room = new RoomDto { Id = roomId, Name = $"Room {roomId}", Type = "Bedroom" };
        _mockService.Setup(s => s.GetById(roomId)).ReturnsAsync(room);

        // Act
        var result = await _controller.GetById(roomId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRoom = Assert.IsType<RoomDto>(okResult.Value);
        Assert.Equal(roomId, returnedRoom.Id);
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAll(null)).ReturnsAsync(new List<RoomDto>());

        // Act
        var result = await _controller.GetAll(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedRooms = Assert.IsAssignableFrom<IEnumerable<RoomDto>>(okResult.Value);
        Assert.Empty(returnedRooms);
    }
}
