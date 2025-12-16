using Moq;
using PostGresAPI.Services;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Services;

// logic tests
public class MeetingroomServiceTests
{
    private readonly Mock<IMeetingroomRepository> _mockRepo;
    private readonly MeetingroomService _service;

    public MeetingroomServiceTests()
    {
        _mockRepo = new Mock<IMeetingroomRepository>();
        _service = new MeetingroomService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsAllMeetingrooms()
    {
        // Arrange
        var meetingrooms = new List<Meetingroom>
        {
            new Meetingroom("Meeting Room 1", 10),
            new Meetingroom("Meeting Room 2", 20)
        };

        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(meetingrooms);

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Meetingroom>());

        // Act
        var result = await _service.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetById_ExistingMeetingroom_ReturnsMeetingroom()
    {
        // Arrange
        var meetingroom = new Meetingroom("Test Meeting Room", 15);
        _mockRepo.Setup(r => r.GetById(1)).ReturnsAsync(meetingroom);

        // Act
        var result = await _service.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Meeting Room", result.Name);
        Assert.Equal(15, result.NumberOfChairs);
    }

    [Fact]
    public async Task GetById_NonExistingMeetingroom_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetById(999)).ReturnsAsync((Meetingroom?)null);

        // Act
        var result = await _service.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Create_ValidMeetingroom_ReturnsCreatedMeetingroom()
    {
        // Arrange
        var createDto = new CreateMeetingroomDto
        {
            Name = "New Meeting Room",
            NumberOfChairs = 25,
            ImagePath = "/images/meeting.jpg"
        };
        var createdMeetingroom = new Meetingroom("New Meeting Room", 25);

        _mockRepo.Setup(r => r.Add(createDto)).ReturnsAsync(createdMeetingroom);

        // Act
        var result = await _service.Create(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Meeting Room", result.Name);
        Assert.Equal(25, result.NumberOfChairs);
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
        var createdMeetingroom = new Meetingroom("Simple Meeting Room", 10);

        _mockRepo.Setup(r => r.Add(createDto)).ReturnsAsync(createdMeetingroom);

        // Act
        var result = await _service.Create(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Simple Meeting Room", result.Name);
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
        var updatedMeetingroom = new Meetingroom("Updated Meeting Room", 30);

        _mockRepo.Setup(r => r.Update(1, updateDto)).ReturnsAsync(updatedMeetingroom);

        // Act
        var result = await _service.Update(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Meeting Room", result.Name);
        Assert.Equal(30, result.NumberOfChairs);
    }

    [Fact]
    public async Task Update_NonExistingMeetingroom_ReturnsNull()
    {
        // Arrange
        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Updated Meeting Room",
            NumberOfChairs = 20
        };
        _mockRepo.Setup(r => r.Update(999, updateDto)).ReturnsAsync((Meetingroom?)null);

        // Act
        var result = await _service.Update(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_ExistingMeetingroom_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(r => r.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _service.Delete(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Delete_NonExistingMeetingroom_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.Delete(999)).ReturnsAsync(false);

        // Act
        var result = await _service.Delete(999);

        // Assert
        Assert.False(result);
    }
}
