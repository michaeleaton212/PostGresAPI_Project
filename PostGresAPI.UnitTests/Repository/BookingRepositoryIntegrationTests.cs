using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Repository;

public class BookingRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BookingRepository _repository;

    public BookingRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BookingRepository(_context);
        
        // Seed some test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add test rooms
        var rooms = new List<Room>
        {
            new Bedroom { Name = "Bedroom 1" },
            new Bedroom { Name = "Bedroom 2" },
            new Meetingroom { Name = "Meeting Room 1", NumberOfChairs = 10 }
        };
        _context.Rooms.AddRange(rooms);
        _context.SaveChanges();
    }

    [Fact]
    public async Task HasOverlap_WithOverlappingBooking_ReturnsTrue()
    {
        // Arrange
        var existingBooking = new Booking(
            1,
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2),
            "Existing"
        );
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        // Act - Check for overlap with time range that overlaps
        var hasOverlap = await _repository.HasOverlap(
            1,
            DateTimeOffset.UtcNow.AddDays(1).AddHours(12),
            DateTimeOffset.UtcNow.AddDays(2).AddHours(12)
        );

        // Assert
        Assert.True(hasOverlap);
    }

    [Fact]
    public async Task HasOverlap_WithoutOverlappingBooking_ReturnsFalse()
    {
        // Arrange
        var existingBooking = new Booking(
            1,
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2),
            "Existing"
        );
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        // Act - Check for overlap with time range that doesn't overlap
        var hasOverlap = await _repository.HasOverlap(
            1,
            DateTimeOffset.UtcNow.AddDays(3),
            DateTimeOffset.UtcNow.AddDays(4)
        );

        // Assert
        Assert.False(hasOverlap);
    }

    [Fact]
    public async Task HasOverlap_DifferentRoom_ReturnsFalse()
    {
        // Arrange
        var existingBooking = new Booking(
            1,
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2),
            "Room 1 Booking"
        );
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        // Act - Check for overlap in different room
        var hasOverlap = await _repository.HasOverlap(
            2,
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2)
        );

        // Assert
        Assert.False(hasOverlap);
    }

    [Fact]
    public async Task HasOverlap_WithExcludeBookingId_IgnoresExcludedBooking()
    {
        // Arrange
        var existingBooking = new Booking(
            1,
            DateTimeOffset.UtcNow.AddDays(1),
            DateTimeOffset.UtcNow.AddDays(2),
            "Existing"
        );
        _context.Bookings.Add(existingBooking);
        await _context.SaveChangesAsync();

        // Detach to get the ID
        _context.Entry(existingBooking).State = EntityState.Detached;
        var bookingId = existingBooking.Id;

        // Act - Check overlap but exclude the existing booking
        var hasOverlap = await _repository.HasOverlap(
            1,
            DateTimeOffset.UtcNow.AddDays(1).AddHours(12),
            DateTimeOffset.UtcNow.AddDays(2).AddHours(12),
            bookingId
        );

        // Assert
        Assert.False(hasOverlap);
    }

    [Fact]
    public async Task GetByRoomId_ReturnsOnlyBookingsForThatRoom()
    {
        // Arrange
        _context.Bookings.Add(new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "Room 1"
        ));
        _context.Bookings.Add(new Booking(
            2,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "Room 2"
        ));
        _context.Bookings.Add(new Booking(
            1,
            DateTimeOffset.UtcNow.AddDays(2),
            DateTimeOffset.UtcNow.AddDays(3),
            "Room 1 Again"
        ));
        await _context.SaveChangesAsync();

        // Act
        var bookings = await _repository.GetByRoomId(1);

        // Assert
        Assert.Equal(2, bookings.Count);
        Assert.All(bookings, b => Assert.Equal(1, b.RoomId));
    }

    [Fact]
    public async Task GetByName_ReturnsBookingsWithMatchingName()
    {
        // Arrange
        _context.Bookings.Add(new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "test@example.com"
        ));
        _context.Bookings.Add(new Booking(
            1,
            DateTimeOffset.UtcNow.AddDays(2),
            DateTimeOffset.UtcNow.AddDays(3),
            "test@example.com"
        ));
        _context.Bookings.Add(new Booking(
            2,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "other@example.com"
        ));
        await _context.SaveChangesAsync();

        // Act
        var bookings = await _repository.GetByName("test@example.com");

        // Assert
        Assert.Equal(2, bookings.Count);
        Assert.All(bookings, b => Assert.Equal("test@example.com", b.Title));
    }

    [Fact]
    public async Task GetByBookingNumber_ReturnsCorrectBooking()
    {
        // Arrange
        var booking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "Test",
            "CUSTOM123"
        );
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBookingNumber("CUSTOM123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("CUSTOM123", result.BookingNumber);
    }

    [Fact]
    public async Task Add_CreatesNewBooking()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            RoomId = 1,
            StartUtc = DateTimeOffset.UtcNow.AddDays(1),
            EndUtc = DateTimeOffset.UtcNow.AddDays(2),
            Title = "New Booking"
        };

        // Act
        var result = await _repository.Add(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.RoomId);
        Assert.Equal("New Booking", result.Title);
        Assert.NotNull(result.BookingNumber);
        
        // Verify it was saved to database
        _context.Entry(result).State = EntityState.Detached;
        var savedBooking = await _context.Bookings.FindAsync(result.Id);
        Assert.NotNull(savedBooking);
    }

    [Fact]
    public async Task Update_UpdatesExistingBooking()
    {
        // Arrange
        var booking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "Original"
        );
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        
        _context.Entry(booking).State = EntityState.Detached;
        var bookingId = booking.Id;

        var newStart = DateTimeOffset.UtcNow.AddDays(2);
        var newEnd = DateTimeOffset.UtcNow.AddDays(3);

        // Act
        var result = await _repository.Update(bookingId, newStart, newEnd, "Updated");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Title);
    }

    [Fact]
    public async Task UpdateStatus_UpdatesBookingStatus()
    {
        // Arrange
        var booking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "Test"
        );
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        
        _context.Entry(booking).State = EntityState.Detached;
        var bookingId = booking.Id;

        // Act
        var result = await _repository.UpdateStatus(bookingId, BookingStatus.CheckedIn);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(BookingStatus.CheckedIn, result.Status);
    }

    [Fact]
    public async Task Delete_RemovesBooking()
    {
        // Arrange
        var booking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "To Delete"
        );
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        
        _context.Entry(booking).State = EntityState.Detached;
        var bookingId = booking.Id;

        // Act
        var result = await _repository.Delete(bookingId);

        // Assert
        Assert.True(result);
        var deletedBooking = await _context.Bookings.FindAsync(bookingId);
        Assert.Null(deletedBooking);
    }

    [Fact]
    public async Task Delete_NonExistingBooking_ReturnsFalse()
    {
        // Act
        var result = await _repository.Delete(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAll_ReturnsSortedByStartTime()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        _context.Bookings.Add(new Booking(
            1,
            now.AddDays(2),
            now.AddDays(3),
            "Third"
        ));
        _context.Bookings.Add(new Booking(
            1,
            now.AddDays(1),
            now.AddDays(2),
            "Second"
        ));
        _context.Bookings.Add(new Booking(
            1,
            now,
            now.AddDays(1),
            "First"
        ));
        await _context.SaveChangesAsync();

        // Act
        var bookings = await _repository.GetAll();

        // Assert
        Assert.Equal(3, bookings.Count);
        Assert.Equal("First", bookings[0].Title);
        Assert.Equal("Second", bookings[1].Title);
        Assert.Equal("Third", bookings[2].Title);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
