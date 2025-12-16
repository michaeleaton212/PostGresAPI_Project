using Moq;
using PostGresAPI.Services;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Services;

public class RoomServiceTests
{
    //logic tests
    private readonly Mock<IRoomRepository> _mockRepo;
    private readonly RoomService _service;

    public RoomServiceTests()
    {
        _mockRepo = new Mock<IRoomRepository>();
        _service = new RoomService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAll_NoFilter_ReturnsAllRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            new Bedroom("Room 1", 2),
            new Meetingroom("Room 2", 10)
        };

        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(rooms);

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_FilterByBedroom_ReturnsOnlyBedrooms()
    {
        // Arrange
        var bedrooms = new List<Bedroom>
        {
            new Bedroom("Bedroom 1", 2)
        };

        _mockRepo.Setup(r => r.GetBedrooms()).ReturnsAsync(bedrooms);

        // Act
        var result = await _service.GetAll("Bedroom");

        // Assert
        Assert.Single(result);
        Assert.Equal("Bedroom", result[0].Type);
    }

    [Fact]
    public async Task GetAll_FilterByMeetingroom_ReturnsOnlyMeetingrooms()
    {
        // Arrange
        var meetingrooms = new List<Meetingroom>
        {
            new Meetingroom("Meeting 1", 10)
        };

        _mockRepo.Setup(r => r.GetMeetingrooms()).ReturnsAsync(meetingrooms);

        // Act
        var result = await _service.GetAll("Meetingroom");

        // Assert
        Assert.Single(result);
        Assert.Equal("Meetingroom", result[0].Type);
    }

    [Fact]
    public async Task GetAll_UnknownType_ReturnsEmptyList()
    {
        // Act
        var result = await _service.GetAll("UnknownType");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_FilterIsCaseInsensitive_ReturnsCorrectRooms()
    {
        // Arrange
        var bedrooms = new List<Bedroom>
        {
            new Bedroom("Bedroom 1", 2)
        };

        _mockRepo.Setup(r => r.GetBedrooms()).ReturnsAsync(bedrooms);

        // Act
        var result = await _service.GetAll("BEDROOM");

        // Assert
        Assert.Single(result);
        Assert.Equal("Bedroom", result[0].Type);
    }

    [Fact]
    public async Task GetById_ExistingRoom_ReturnsRoom()
    {
        // Arrange
        var room = new Bedroom("Test Room", 2);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(room);

        // Act
        var result = await _service.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Room", result.Name);
    }

    [Fact]
    public async Task GetById_NonExistingRoom_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetById(999)).ReturnsAsync((Room?)null);

        // Act
        var result = await _service.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateMeetingroom_ValidData_ReturnsCreatedRoom()
    {
        // Arrange
        var dto = new CreateMeetingroomDto
        {
            Name = "New Meeting Room",
            NumberOfChairs = 20,
            ImagePath = "/images/meeting.jpg"
        };

        var createdRoom = new Meetingroom("New Meeting Room", 20);

        _mockRepo.Setup(r => r.Add(It.IsAny<Room>())).ReturnsAsync(createdRoom);

        // Act
        var result = await _service.CreateMeetingroom(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Meeting Room", result.Name);
        Assert.Equal(20, result.NumberOfChairs);
    }

    [Fact]
    public async Task CreateMeetingroom_WithoutImage_CreatesSuccessfully()
    {
        // Arrange
        var dto = new CreateMeetingroomDto
        {
            Name = "Meeting Room",
            NumberOfChairs = 15
        };

        var createdRoom = new Meetingroom("Meeting Room", 15);

        _mockRepo.Setup(r => r.Add(It.IsAny<Room>())).ReturnsAsync(createdRoom);

        // Act
        var result = await _service.CreateMeetingroom(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Meeting Room", result.Name);
    }

    [Fact]
    public async Task CreateBedroom_ValidData_ReturnsCreatedRoom()
    {
        // Arrange
        var dto = new CreateBedroomDto
        {
            Name = "New Bedroom",
            NumberOfBeds = 2,
            ImagePath = "/images/bedroom.jpg"
        };

        var createdRoom = new Bedroom("New Bedroom", 2);

        _mockRepo.Setup(r => r.Add(It.IsAny<Room>())).ReturnsAsync(createdRoom);

        // Act
        var result = await _service.CreateBedroom(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Bedroom", result.Name);
        Assert.Equal("Bedroom", result.Type);
    }

    [Fact]
    public async Task UpdateName_ExistingRoom_ReturnsUpdatedRoom()
    {
        // Arrange
        var updatedRoom = new Bedroom("Updated Name", 2);
        _mockRepo.Setup(r => r.UpdateName(1, "Updated Name")).ReturnsAsync(updatedRoom);

        // Act
        var result = await _service.UpdateName(1, "Updated Name");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task UpdateName_NonExistingRoom_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.UpdateName(999, "New Name")).ReturnsAsync((Room?)null);

        // Act
        var result = await _service.UpdateName(999, "New Name");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateImage_ExistingRoom_ReturnsUpdatedRoom()
    {
        // Arrange
        var room = new Bedroom("Room", 2);
        room.SetImagePath("/new-image.jpg");
        _mockRepo.Setup(r => r.UpdateImage(1, "/new-image.jpg")).ReturnsAsync(room);

        // Act
        var result = await _service.UpdateImage(1, "/new-image.jpg");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("/new-image.jpg", result.image);
    }

    [Fact]
    public async Task UpdateImage_NonExistingRoom_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.UpdateImage(999, "/image.jpg")).ReturnsAsync((Room?)null);

        // Act
        var result = await _service.UpdateImage(999, "/image.jpg");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ExistingRoom_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(r => r.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _service.Delete(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Delete_NonExistingRoom_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.Delete(999)).ReturnsAsync(false);

        // Act
        var result = await _service.Delete(999);

        // Assert
        Assert.False(result);
    }
}
