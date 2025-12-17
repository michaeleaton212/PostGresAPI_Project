using PostGresAPI.Extensions;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Extensions;

public class BookingExtensionsTests
{
    [Fact]
    public async Task ToDto_ValidBooking_ReturnsMappedDto()
    {
        // Arrange
        var startTime = new DateTimeOffset(2024, 6, 1, 10, 0, 0, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
        var booking = new Booking(1, startTime, endTime, "Test Meeting", "ABC12345");

        // Act
        var result = booking.ToDto();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.RoomId);
        Assert.Equal(startTime, result.StartTime);
        Assert.Equal(endTime, result.EndTime);
        Assert.Equal("Test Meeting", result.Title);
        Assert.Equal("ABC12345", result.BookingNumber);
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task ToEntity_ValidCreateDto_ReturnsBookingWithGeneratedNumber()
    {
        // Arrange
        var startTime = new DateTimeOffset(2024, 6, 1, 10, 0, 0, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
        var dto = new CreateBookingDto(1, startTime, endTime, "New Booking");

        // Act
        var result = dto.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.RoomId);
        Assert.Equal(startTime, result.StartTime);
        Assert.Equal(endTime, result.EndTime);
        Assert.Equal("New Booking", result.Title);
        Assert.NotNull(result.BookingNumber);
        Assert.Equal(8, result.BookingNumber.Length);
        Assert.Matches("^[A-Z0-9]{8}$", result.BookingNumber);
    }

    [Fact]
    public async Task UpdateEntity_ValidUpdateDto_UpdatesBookingProperties()
    {
        // Arrange
        var oldStart = new DateTimeOffset(2024, 6, 1, 10, 0, 0, TimeSpan.Zero);
        var oldEnd = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
        var booking = new Booking(1, oldStart, oldEnd, "Old Title", "OLD12345");

        var newStart = new DateTimeOffset(2024, 6, 2, 14, 0, 0, TimeSpan.Zero);
        var newEnd = new DateTimeOffset(2024, 6, 2, 16, 0, 0, TimeSpan.Zero);
        var updateDto = new UpdateBookingDto(newStart, newEnd, "Updated Title");

        // Act
        updateDto.UpdateEntity(booking);

        // Assert
        Assert.Equal(newStart, booking.StartTime);
        Assert.Equal(newEnd, booking.EndTime);
        Assert.Equal("Updated Title", booking.Title);
    }

    [Fact]
    public async Task ApplyUpdate_ValidParameters_UpdatesBookingTimes()
    {
        // Arrange
        var oldStart = new DateTimeOffset(2024, 6, 1, 10, 0, 0, TimeSpan.Zero);
        var oldEnd = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero);
        var booking = new Booking(1, oldStart, oldEnd, "Meeting", "TEST1234");

        var newStart = new DateTimeOffset(2024, 6, 3, 9, 0, 0, TimeSpan.Zero);
        var newEnd = new DateTimeOffset(2024, 6, 3, 11, 0, 0, TimeSpan.Zero);

        // Act
        booking.ApplyUpdate(newStart, newEnd, "Updated Meeting");

        // Assert
        Assert.Equal(newStart, booking.StartTime);
        Assert.Equal(newEnd, booking.EndTime);
        Assert.Equal("Updated Meeting", booking.Title);
    }

    [Fact]
    public async Task ApplyStatusUpdate_ValidStatus_UpdatesBookingStatus()
    {
        // Arrange
        var booking = new Booking(1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(2), "Meeting", "STAT1234");
        Assert.Equal(BookingStatus.Pending, booking.Status);

        // Act
        booking.ApplyStatusUpdate(BookingStatus.Expired);

        // Assert
        Assert.Equal(BookingStatus.Expired, booking.Status);
    }

    [Fact]
    public async Task ToDto_ExpiredBooking_ReturnsDtoWithExpiredStatus()
    {
        // Arrange
        var booking = new Booking(1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(2), "Past Meeting", "EXP12345");
        booking.ApplyStatusUpdate(BookingStatus.Expired);

        // Act
        var result = booking.ToDto();

        // Assert
        Assert.Equal("Expired", result.Status);
    }

    [Fact]
    public async Task ToEntity_MultipleCreations_GenerateUniqueBookingNumbers()
    {
        // Arrange
        var dto = new CreateBookingDto(1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1), "Test");

        // Act
        var booking1 = dto.ToEntity();
        var booking2 = dto.ToEntity();
        var booking3 = dto.ToEntity();

        // Assert
        Assert.NotEqual(booking1.BookingNumber, booking2.BookingNumber);
        Assert.NotEqual(booking2.BookingNumber, booking3.BookingNumber);
        Assert.NotEqual(booking1.BookingNumber, booking3.BookingNumber);
    }
}
