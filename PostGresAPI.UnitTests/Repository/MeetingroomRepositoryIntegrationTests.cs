using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Repository;

// test crude operations with temporary db in ram
public class MeetingroomRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MeetingroomRepository _repository;

    public MeetingroomRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new MeetingroomRepository(_context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllMeetingrooms()
    {
        // Arrange
        _context.Meetingrooms.Add(new Meetingroom("Meeting Room 1", 10));
        _context.Meetingrooms.Add(new Meetingroom("Meeting Room 2", 20));
        await _context.SaveChangesAsync();

        // Act
        var meetingrooms = await _repository.GetAll();

        // Assert
        Assert.Equal(2, meetingrooms.Count);
    }

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var meetingrooms = await _repository.GetAll();

        // Assert
        Assert.Empty(meetingrooms);
    }

    [Fact]
    public async Task GetAll_ReturnsSortedById()
    {
        // Arrange
        var meeting3 = new Meetingroom("Meeting Room 3", 30);
        var meeting1 = new Meetingroom("Meeting Room 1", 10);
        var meeting2 = new Meetingroom("Meeting Room 2", 20);

        _context.Meetingrooms.Add(meeting3);
        _context.Meetingrooms.Add(meeting1);
        _context.Meetingrooms.Add(meeting2);
        await _context.SaveChangesAsync();

        // Act
        var meetingrooms = await _repository.GetAll();

        // Assert
        Assert.Equal(3, meetingrooms.Count);
        // Verify they are sorted by Id
        for (int i = 0; i < meetingrooms.Count - 1; i++)
        {
            Assert.True(meetingrooms[i].Id < meetingrooms[i + 1].Id);
        }
    }

    [Fact]
    public async Task GetById_ExistingMeetingroom_ReturnsMeetingroom()
    {
        // Arrange
        var meetingroom = new Meetingroom("Test Meeting Room", 15);
        _context.Meetingrooms.Add(meetingroom);
        await _context.SaveChangesAsync();
        var meetingroomId = meetingroom.Id;
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetById(meetingroomId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(meetingroomId, result.Id);
        Assert.Equal("Test Meeting Room", result.Name);
        Assert.Equal(15, result.NumberOfChairs);
    }

    [Fact]
    public async Task GetById_NonExistingMeetingroom_ReturnsNull()
    {
        // Act
        var result = await _repository.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CreatesMeetingroom()
    {
        // Arrange
        var createDto = new CreateMeetingroomDto
        {
            Name = "New Meeting Room",
            NumberOfChairs = 25,
            ImagePath = "/images/meeting.jpg"
        };

        // Act
        var result = await _repository.Add(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Meeting Room", result.Name);
        Assert.Equal(25, result.NumberOfChairs);

        // Clear and verify
        _context.ChangeTracker.Clear();
        var savedMeetingroom = await _context.Meetingrooms.FirstOrDefaultAsync(m => m.Id == result.Id);
        Assert.NotNull(savedMeetingroom);
    }

    [Fact]
    public async Task Update_UpdatesMeetingroom()
    {
        // Arrange
        var meetingroom = new Meetingroom("Original Meeting Room", 10);
        _context.Meetingrooms.Add(meetingroom);
        await _context.SaveChangesAsync();
        var meetingroomId = meetingroom.Id;
        _context.ChangeTracker.Clear();

        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Updated Meeting Room",
            NumberOfChairs = 30,
            ImagePath = "/images/updated.jpg"
        };

        // Act
        var result = await _repository.Update(meetingroomId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Meeting Room", result.Name);
        Assert.Equal(30, result.NumberOfChairs);

        // Verify in database
        _context.ChangeTracker.Clear();
        var updatedMeetingroom = await _context.Meetingrooms.FirstOrDefaultAsync(m => m.Id == meetingroomId);
        Assert.Equal("Updated Meeting Room", updatedMeetingroom?.Name);
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

        // Act
        var result = await _repository.Update(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_RemovesMeetingroom()
    {
        // Arrange
        var meetingroom = new Meetingroom("To Delete", 10);
        _context.Meetingrooms.Add(meetingroom);
        await _context.SaveChangesAsync();
        var meetingroomId = meetingroom.Id;
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.Delete(meetingroomId);

        // Assert
        Assert.True(result);

        // Verify it was deleted
        var deletedMeetingroom = await _context.Meetingrooms.FirstOrDefaultAsync(m => m.Id == meetingroomId);
        Assert.Null(deletedMeetingroom);
    }

    [Fact]
    public async Task Delete_NonExistingMeetingroom_ReturnsFalse()
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
