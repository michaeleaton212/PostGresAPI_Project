using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Repository;
using PostGresAPI.Models;

namespace PostGresAPI.UnitTests.Repository;

public class RoomRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RoomRepository _repository;

    public RoomRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new RoomRepository(_context);
    }

    [Fact]
    public async Task GetAll_ReturnsAllRooms()
    {
        // Arrange
        _context.Rooms.Add(new Bedroom("Bedroom 1", 2));
        _context.Rooms.Add(new Meetingroom("Meeting 1", 10));
        await _context.SaveChangesAsync();

        // Act
        var rooms = await _repository.GetAll();

        // Assert
        Assert.Equal(2, rooms.Count);
    }

    [Fact]
    public async Task GetBedrooms_ReturnsOnlyBedrooms()
    {
        // Arrange
        _context.Rooms.Add(new Bedroom("Bedroom 1", 2));
        _context.Rooms.Add(new Bedroom("Bedroom 2", 1));
        _context.Rooms.Add(new Meetingroom("Meeting 1", 10));
        await _context.SaveChangesAsync();

        // Act
        var bedrooms = await _repository.GetBedrooms();

        // Assert
        Assert.Equal(2, bedrooms.Count);
        Assert.All(bedrooms, b => Assert.IsType<Bedroom>(b));
    }

    [Fact]
    public async Task GetMeetingrooms_ReturnsOnlyMeetingrooms()
    {
        // Arrange
        _context.Rooms.Add(new Bedroom("Bedroom 1", 2));
        _context.Rooms.Add(new Meetingroom("Meeting 1", 10));
        _context.Rooms.Add(new Meetingroom("Meeting 2", 20));
        await _context.SaveChangesAsync();

        // Act
        var meetingrooms = await _repository.GetMeetingrooms();

        // Assert
        Assert.Equal(2, meetingrooms.Count);
        Assert.All(meetingrooms, m => Assert.IsType<Meetingroom>(m));
    }

    [Fact]
    public async Task GetById_ExistingRoom_ReturnsRoom()
    {
        // Arrange
        var room = new Bedroom("Test Room", 2);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetById(room.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(room.Id, result.Id);
        Assert.Equal("Test Room", result.Name);
    }

    [Fact]
    public async Task GetById_NonExistingRoom_ReturnsNull()
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
        var bedroom = new Bedroom("New Bedroom", 2);

        // Act
        var result = await _repository.Add(bedroom);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Bedroom", result.Name);
        
        // Verify it was saved
        var savedRoom = await _context.Rooms.FindAsync(result.Id);
        Assert.NotNull(savedRoom);
    }

    [Fact]
    public async Task Add_CreatesMeetingroom()
    {
        // Arrange
        var meetingroom = new Meetingroom("New Meeting Room", 15);

        // Act
        var result = await _repository.Add(meetingroom);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Meetingroom>(result);
        var meetingroomResult = (Meetingroom)result;
        Assert.Equal(15, meetingroomResult.NumberOfChairs);
    }

    [Fact]
    public async Task UpdateName_UpdatesRoomName()
    {
        // Arrange
        var room = new Bedroom("Original Name", 2);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UpdateName(room.Id, "Updated Name");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        
        // Verify in database
        var updatedRoom = await _context.Rooms.FindAsync(room.Id);
        Assert.Equal("Updated Name", updatedRoom?.Name);
    }

    [Fact]
    public async Task UpdateName_NonExistingRoom_ReturnsNull()
    {
        // Act
        var result = await _repository.UpdateName(999, "New Name");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateImage_UpdatesRoomImage()
    {
        // Arrange
        var room = new Bedroom("Test Room", 2);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UpdateImage(room.Id, "/images/new-image.jpg");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("/images/new-image.jpg", result.ImagePath);
    }

    [Fact]
    public async Task UpdateImage_NonExistingRoom_ReturnsNull()
    {
        // Act
        var result = await _repository.UpdateImage(999, "/image.jpg");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_RemovesRoom()
    {
        // Arrange
        var room = new Bedroom("To Delete", 2);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        var roomId = room.Id;

        // Act
        var result = await _repository.Delete(roomId);

        // Assert
        Assert.True(result);
        
        // Verify it was deleted
        var deletedRoom = await _context.Rooms.FindAsync(roomId);
        Assert.Null(deletedRoom);
    }

    [Fact]
    public async Task Delete_NonExistingRoom_ReturnsFalse()
    {
        // Act
        var result = await _repository.Delete(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Exists_ExistingRoom_ReturnsTrue()
    {
        // Arrange
        var room = new Bedroom("Test Room", 2);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.Exists(room.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Exists_NonExistingRoom_ReturnsFalse()
    {
        // Act
        var result = await _repository.Exists(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAll_ReturnsSortedById()
    {
        // Arrange
        var room3 = new Bedroom("Room 3", 3);
        var room1 = new Bedroom("Room 1", 1);
        var room2 = new Bedroom("Room 2", 2);
        
        _context.Rooms.Add(room3);
        _context.Rooms.Add(room1);
        _context.Rooms.Add(room2);
        await _context.SaveChangesAsync();

        // Act
        var rooms = await _repository.GetAll();

        // Assert
        Assert.Equal(3, rooms.Count);
        // Verify they are sorted by Id
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Assert.True(rooms[i].Id < rooms[i + 1].Id);
        }
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
