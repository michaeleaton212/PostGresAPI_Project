using Moq;
using PostGresAPI.Services;
using PostGresAPI.Repository;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly Mock<IRoomRepository> _mockRoomRepo;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockRoomRepo = new Mock<IRoomRepository>();
        _service = new BookingService(_mockBookingRepo.Object, _mockRoomRepo.Object);
    }

    [Fact]
    public async Task Create_ValidBooking_ReturnsSuccess()
    {
        // Arrange
        var dto = new CreateBookingDto(
            RoomId: 1,
            StartUtc: DateTimeOffset.UtcNow.AddDays(1),
            EndUtc: DateTimeOffset.UtcNow.AddDays(2),
            Title: "Test Booking"
        );

        _mockRoomRepo.Setup(r => r.Exists(1)).ReturnsAsync(true);
        _mockBookingRepo.Setup(r => r.HasOverlap(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()))
            .ReturnsAsync(false);
        _mockBookingRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Booking>());
        _mockBookingRepo.Setup(r => r.Add(It.IsAny<CreateBookingDto>()))
            .ReturnsAsync(new Booking(1, dto.StartUtc, dto.EndUtc, dto.Title, "BK001"));

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.True(result.Ok);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
    }

    [Fact]
    public async Task Create_StartAfterEnd_ReturnsError()
    {
        // Arrange
        var dto = new CreateBookingDto(
            RoomId: 1,
            StartUtc: DateTimeOffset.UtcNow.AddDays(2),
            EndUtc: DateTimeOffset.UtcNow.AddDays(1),
            Title: "Invalid Booking"
        );

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Start must be before End.", result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task Create_RoomDoesNotExist_ReturnsError()
    {
        // Arrange
        var dto = new CreateBookingDto(
            RoomId: 999,
            StartUtc: DateTimeOffset.UtcNow.AddDays(1),
            EndUtc: DateTimeOffset.UtcNow.AddDays(2),
            Title: "Test Booking"
        );

        _mockRoomRepo.Setup(r => r.Exists(999)).ReturnsAsync(false);

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Room 999 not found.", result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task Create_TimeRangeOverlaps_ReturnsError()
    {
        // Arrange
        var dto = new CreateBookingDto(
            RoomId: 1,
            StartUtc: DateTimeOffset.UtcNow.AddDays(1),
            EndUtc: DateTimeOffset.UtcNow.AddDays(2),
            Title: "Test Booking"
        );

        _mockRoomRepo.Setup(r => r.Exists(1)).ReturnsAsync(true);
        _mockBookingRepo.Setup(r => r.HasOverlap(1, dto.StartUtc, dto.EndUtc))
            .ReturnsAsync(true);

        // Act
        var result = await _service.Create(dto);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Time range already booked.", result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task Update_ValidBooking_ReturnsSuccess()
    {
        // Arrange
        var updateDto = new UpdateBookingDto(
            StartUtc: DateTimeOffset.UtcNow.AddDays(1),
            EndUtc: DateTimeOffset.UtcNow.AddDays(2),
            Title: "Updated Booking"
        );

        var existingBooking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "Original",
            "BK001"
        );

        _mockBookingRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Booking>());
        _mockBookingRepo.Setup(r => r.GetById(1)).ReturnsAsync(existingBooking);
        _mockBookingRepo.Setup(r => r.HasOverlap(1, updateDto.StartUtc, updateDto.EndUtc, 1))
            .ReturnsAsync(false);
        _mockBookingRepo.Setup(r => r.Update(1, updateDto.StartUtc, updateDto.EndUtc, updateDto.Title))
            .ReturnsAsync(existingBooking);

        // Act
        var result = await _service.Update(1, updateDto);

        // Assert
        Assert.True(result.Ok);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
    }

    [Fact]
    public async Task Update_StartAfterEnd_ReturnsError()
    {
        // Arrange
        var updateDto = new UpdateBookingDto(
            StartUtc: DateTimeOffset.UtcNow.AddDays(2),
            EndUtc: DateTimeOffset.UtcNow.AddDays(1),
            Title: "Invalid Update"
        );

        // Act
        var result = await _service.Update(1, updateDto);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Start must be before End.", result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task Update_BookingNotFound_ReturnsError()
    {
        // Arrange
        var updateDto = new UpdateBookingDto(
            StartUtc: DateTimeOffset.UtcNow.AddDays(1),
            EndUtc: DateTimeOffset.UtcNow.AddDays(2),
            Title: "Update"
        );

        _mockBookingRepo.Setup(r => r.GetById(999)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.Update(999, updateDto);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Booking not found.", result.Error);
    }

    [Fact]
    public async Task UpdateStatus_ValidStatus_ReturnsSuccess()
    {
        // Arrange
        var statusDto = new UpdateBookingStatusDto(Status: "CheckedIn");
        var existingBooking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "Test",
            "BK001"
        );

        _mockBookingRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Booking>());
        _mockBookingRepo.Setup(r => r.GetById(1)).ReturnsAsync(existingBooking);
        _mockBookingRepo.Setup(r => r.UpdateStatus(1, BookingStatus.CheckedIn))
            .ReturnsAsync(existingBooking);

        // Act
        var result = await _service.UpdateStatus(1, statusDto);

        // Assert
        Assert.True(result.Ok);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task UpdateStatus_InvalidStatus_ReturnsError()
    {
        // Arrange
        var statusDto = new UpdateBookingStatusDto(Status: "InvalidStatus");
        var existingBooking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "Test",
            "BK001"
        );

        _mockBookingRepo.Setup(r => r.GetById(1)).ReturnsAsync(existingBooking);

        // Act
        var result = await _service.UpdateStatus(1, statusDto);

        // Assert
        Assert.False(result.Ok);
        Assert.Contains("Invalid status", result.Error);
    }

    [Fact]
    public async Task UpdateStatus_BookingNotFound_ReturnsError()
    {
        // Arrange
        var statusDto = new UpdateBookingStatusDto(Status: "CheckedIn");
        _mockBookingRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Booking>());
        _mockBookingRepo.Setup(r => r.GetById(999)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.UpdateStatus(999, statusDto);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Booking not found.", result.Error);
    }

    [Fact]
    public async Task Delete_ExistingBooking_ReturnsSuccess()
    {
        // Arrange
        _mockBookingRepo.Setup(r => r.Delete(1)).ReturnsAsync(true);

        // Act
        var result = await _service.Delete(1);

        // Assert
        Assert.True(result.Ok);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task Delete_NonExistingBooking_ReturnsError()
    {
        // Arrange
        _mockBookingRepo.Setup(r => r.Delete(999)).ReturnsAsync(false);

        // Act
        var result = await _service.Delete(999);

        // Assert
        Assert.False(result.Ok);
        Assert.Equal("Booking not found.", result.Error);
    }

    [Fact]
    public async Task GetBookingIdsByCredentials_ValidCredentials_ReturnsBookingIds()
    {
        // Arrange
        var booking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "test@example.com",
            "BK123"
        );

        _mockBookingRepo.Setup(r => r.GetByBookingNumber("BK123")).ReturnsAsync(booking);
        _mockBookingRepo.Setup(r => r.GetByName("test@example.com"))
            .ReturnsAsync(new List<Booking> { booking });

        // Act
        var result = await _service.GetBookingIdsByCredentials("BK123", "test@example.com");

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetBookingIdsByCredentials_InvalidBookingNumber_ReturnsEmpty()
    {
        // Arrange
        _mockBookingRepo.Setup(r => r.GetByBookingNumber("INVALID")).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetBookingIdsByCredentials("INVALID", "test@example.com");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookingIdsByCredentials_NameMismatch_ReturnsEmpty()
    {
        // Arrange
        var booking = new Booking(
            1,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(1),
            "test@example.com",
            "BK123"
        );

        _mockBookingRepo.Setup(r => r.GetByBookingNumber("BK123")).ReturnsAsync(booking);

        // Act
        var result = await _service.GetBookingIdsByCredentials("BK123", "wrong@example.com");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookingIdsByCredentials_EmptyCredentials_ReturnsEmpty()
    {
        // Act
        var result = await _service.GetBookingIdsByCredentials("", "");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void IsActive_BookingIsActive_ReturnsTrue()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var booking = new BookingDto(
            1,
            1,
            now.AddHours(-1),
            now.AddHours(1),
            "Test",
            "BK001",
            "Pending"
        );

        // Act
        var result = _service.IsActive(booking, now);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsActive_BookingNotYetStarted_ReturnsFalse()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var booking = new BookingDto(
            1,
            1,
            now.AddHours(1),
            now.AddHours(2),
            "Test",
            "BK001",
            "Pending"
        );

        // Act
        var result = _service.IsActive(booking, now);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsActive_BookingAlreadyEnded_ReturnsFalse()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var booking = new BookingDto(
            1,
            1,
            now.AddHours(-2),
            now.AddHours(-1),
            "Test",
            "BK001",
            "Pending"
        );

        // Act
        var result = _service.IsActive(booking, now);

        // Assert
        Assert.False(result);
    }
}
