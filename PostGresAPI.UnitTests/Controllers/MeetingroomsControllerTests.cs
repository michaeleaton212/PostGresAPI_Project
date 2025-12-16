using Microsoft.AspNetCore.Mvc;
using Moq;
using PostGresAPI.Controllers;
using PostGresAPI.Services;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Controllers;

// tests HTTP-Result-Typ, Data in Result, and mapping of the data (Output test)
// defines service output in mock and verifies controller output
public class MeetingroomsControllerTests
{
    private readonly Mock<IMeetingroomService> _mockService;
    private readonly MeetingroomsController _controller;

    public MeetingroomsControllerTests()
    {
        _mockService = new Mock<IMeetingroomService>();
        _controller = new MeetingroomsController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllMeetingrooms()
    {
        // Arrange
        var meetingrooms = new List<MeetingroomDto>
        {
            new MeetingroomDto(1, "Meeting Room 1", 10, "/images/meeting1.jpg"),
            new MeetingroomDto(2, "Meeting Room 2", 20, "/images/meeting2.jpg")
        };

        _mockService.Setup(s => s.GetAll()).ReturnsAsync(meetingrooms);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMeetingrooms = Assert.IsAssignableFrom<IEnumerable<MeetingroomDto>>(okResult.Value);
        Assert.Equal(2, returnedMeetingrooms.Count());
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAll()).ReturnsAsync(new List<MeetingroomDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMeetingrooms = Assert.IsAssignableFrom<IEnumerable<MeetingroomDto>>(okResult.Value);
        Assert.Empty(returnedMeetingrooms);
    }

    [Fact]
    public async Task GetById_ExistingMeetingroom_ReturnsMeetingroom()
    {
        // Arrange
        var meetingroom = new MeetingroomDto(1, "Test Meeting Room", 15, "/images/test.jpg");
        _mockService.Setup(s => s.GetById(1)).ReturnsAsync(meetingroom);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMeetingroom = Assert.IsType<MeetingroomDto>(okResult.Value);
        Assert.Equal(1, returnedMeetingroom.Id);
        Assert.Equal("Test Meeting Room", returnedMeetingroom.Name);
        Assert.Equal(15, returnedMeetingroom.NumberOfChairs);
    }

    [Fact]
    public async Task GetById_NonExistingMeetingroom_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetById(999)).ReturnsAsync((MeetingroomDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task GetById_VariousIds_ReturnsCorrectMeetingroom(int meetingroomId)
    {
        // Arrange
        var meetingroom = new MeetingroomDto(meetingroomId, $"Meeting Room {meetingroomId}", 10, null);
        _mockService.Setup(s => s.GetById(meetingroomId)).ReturnsAsync(meetingroom);

        // Act
        var result = await _controller.GetById(meetingroomId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMeetingroom = Assert.IsType<MeetingroomDto>(okResult.Value);
        Assert.Equal(meetingroomId, returnedMeetingroom.Id);
    }

    [Fact]
    public async Task Create_ValidMeetingroom_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateMeetingroomDto
        {
            Name = "New Meeting Room",
            NumberOfChairs = 25,
            ImagePath = "/images/new.jpg"
        };
        var createdMeetingroom = new MeetingroomDto(1, "New Meeting Room", 25, "/images/new.jpg");

        _mockService.Setup(s => s.Create(createDto)).ReturnsAsync(createdMeetingroom);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedMeetingroom = Assert.IsType<MeetingroomDto>(createdResult.Value);
        Assert.Equal(1, returnedMeetingroom.Id);
        Assert.Equal("New Meeting Room", returnedMeetingroom.Name);
        Assert.Equal(25, returnedMeetingroom.NumberOfChairs);
    }

    [Fact]
    public async Task Create_WithoutImage_CreatesSuccessfully()
    {
        // Arrange
        var createDto = new CreateMeetingroomDto
        {
            Name = "Simple Meeting Room",
            NumberOfChairs = 10
        };
        var createdMeetingroom = new MeetingroomDto(1, "Simple Meeting Room", 10, null);

        _mockService.Setup(s => s.Create(createDto)).ReturnsAsync(createdMeetingroom);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedMeetingroom = Assert.IsType<MeetingroomDto>(createdResult.Value);
        Assert.Equal("Simple Meeting Room", returnedMeetingroom.Name);
    }

    [Fact]
    public async Task Update_ExistingMeetingroom_ReturnsUpdatedMeetingroom()
    {
        // Arrange
        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Updated Meeting Room",
            NumberOfChairs = 30,
            ImagePath = "/images/updated.jpg"
        };
        var updatedMeetingroom = new MeetingroomDto(1, "Updated Meeting Room", 30, "/images/updated.jpg");

        _mockService.Setup(s => s.Update(1, updateDto)).ReturnsAsync(updatedMeetingroom);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMeetingroom = Assert.IsType<MeetingroomDto>(okResult.Value);
        Assert.Equal("Updated Meeting Room", returnedMeetingroom.Name);
        Assert.Equal(30, returnedMeetingroom.NumberOfChairs);
    }

    [Fact]
    public async Task Update_NonExistingMeetingroom_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Updated Meeting Room",
            NumberOfChairs = 20
        };
        _mockService.Setup(s => s.Update(999, updateDto)).ReturnsAsync((MeetingroomDto?)null);

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Delete_ExistingMeetingroom_ReturnsNoContent()
    {
        // Arrange
        _mockService.Setup(s => s.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NonExistingMeetingroom_ReturnsNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.Delete(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
