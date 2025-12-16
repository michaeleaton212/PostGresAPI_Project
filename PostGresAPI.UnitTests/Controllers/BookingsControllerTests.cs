using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PostGresAPI.Controllers;
using PostGresAPI.Services;
using PostGresAPI.Auth;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Controllers;

public class BookingsControllerTests
{
    private readonly Mock<IBookingService> _mockService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly BookingsController _controller;

    public BookingsControllerTests()
    {
        _mockService = new Mock<IBookingService>();
        _mockTokenService = new Mock<ITokenService>();
        _controller = new BookingsController(_mockService.Object, _mockTokenService.Object);
        
        // Setup HttpContext for header access
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Create_ValidBooking_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateBookingDto(
            RoomId: 1,
            StartUtc: DateTimeOffset.UtcNow.AddDays(1),
            EndUtc: DateTimeOffset.UtcNow.AddDays(2),
            Title: "Test"
        );

        var createdBooking = new BookingDto(
            1,
            1,
            dto.StartUtc,
            dto.EndUtc,
            dto.Title,
            "BK001",
            "Pending"
        );

        _mockService.Setup(s => s.Create(dto))
            .ReturnsAsync((true, null, createdBooking));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBooking = Assert.IsType<BookingDto>(actionResult.Value);
        Assert.Equal(1, returnedBooking.Id);
    }

    [Fact]
    public async Task Create_InvalidBooking_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateBookingDto(
            RoomId: 1,
            StartUtc: DateTimeOffset.UtcNow.AddDays(2),
            EndUtc: DateTimeOffset.UtcNow.AddDays(1),
            Title: "Test"
        );

        _mockService.Setup(s => s.Create(dto))
            .ReturnsAsync((false, "Start must be before End.", null));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task Create_RoomNotFound_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateBookingDto(
            RoomId: 999,
            StartUtc: DateTimeOffset.UtcNow.AddDays(1),
            EndUtc: DateTimeOffset.UtcNow.AddDays(2),
            Title: "Test"
        );

        _mockService.Setup(s => s.Create(dto))
            .ReturnsAsync((false, "Room 999 not found.", null));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginDto = new LoginRequestDto(
            BookingNumber: "BK123",
            Name: "test@example.com"
        );

        var bookingIds = new List<int> { 1, 2 };
        _mockService.Setup(s => s.GetBookingIdsByCredentials("BK123", "test@example.com"))
            .ReturnsAsync(bookingIds);
        _mockTokenService.Setup(t => t.Create(bookingIds, It.IsAny<DateTimeOffset>()))
            .Returns("test-token");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponseDto>(okResult.Value);
        Assert.Equal("test-token", response.Token);
        Assert.Equal(2, response.BookingIds.Count);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginRequestDto(
            BookingNumber: "INVALID",
            Name: "test@example.com"
        );

        _mockService.Setup(s => s.GetBookingIdsByCredentials("INVALID", "test@example.com"))
            .ReturnsAsync(new List<int>());

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_EmptyBookingIds_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginRequestDto(
            BookingNumber: "BK123",
            Name: "test@example.com"
        );

        _mockService.Setup(s => s.GetBookingIdsByCredentials("BK123", "test@example.com"))
            .ReturnsAsync((List<int>?)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByRoomId_ValidRoomId_ReturnsBookings()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var bookings = new List<BookingDto>
        {
            new BookingDto(1, 1, now, now.AddDays(1), "Test 1", "BK001", "Pending"),
            new BookingDto(2, 1, now.AddDays(2), now.AddDays(3), "Test 2", "BK002", "Pending")
        };

        _mockService.Setup(s => s.GetByRoomId(1)).ReturnsAsync(bookings);

        // Act
        var result = await _controller.GetByRoomId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBookings = Assert.IsAssignableFrom<IEnumerable<BookingDto>>(okResult.Value);
        Assert.Equal(2, returnedBookings.Count());
    }

    [Fact]
    public async Task GetByRoomId_NoBookings_ReturnsEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetByRoomId(999)).ReturnsAsync(new List<BookingDto>());

        // Act
        var result = await _controller.GetByRoomId(999);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBookings = Assert.IsAssignableFrom<IEnumerable<BookingDto>>(okResult.Value);
        Assert.Empty(returnedBookings);
    }

    [Fact]
    public async Task GetAll_WithValidToken_ReturnsUserBookings()
    {
        // Arrange
        var bookingIds = new List<int> { 1, 2 };
        var now = DateTimeOffset.UtcNow;
        var bookings = new List<BookingDto>
        {
            new BookingDto(1, 1, now, now.AddDays(1), "Test 1", "BK001", "Pending"),
            new BookingDto(2, 1, now.AddDays(2), now.AddDays(3), "Test 2", "BK002", "Pending")
        };

        _controller.ControllerContext.HttpContext.Request.Headers["X-Login-Token"] = "valid-token";
        
        _mockTokenService.Setup(t => t.TryValidate("valid-token", out It.Ref<List<int>>.IsAny))
            .Callback(new TryValidateDelegate((string token, out List<int> ids) => 
            {
                ids = bookingIds;
            }))
            .Returns(true);
        
        _mockService.Setup(s => s.GetByIds(bookingIds)).ReturnsAsync(bookings);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBookings = Assert.IsAssignableFrom<IEnumerable<BookingDto>>(okResult.Value);
        Assert.Equal(2, returnedBookings.Count());
    }

    [Fact]
    public async Task GetAll_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        _mockTokenService.Setup(t => t.TryValidate(It.IsAny<string>(), out It.Ref<List<int>>.IsAny))
            .Returns(false);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_WithValidTokenAndAuthorizedBooking_ReturnsBooking()
    {
        // Arrange
        var bookingIds = new List<int> { 1 };
        var now = DateTimeOffset.UtcNow;
        var booking = new BookingDto(
            1,
            1,
            now,
            now.AddDays(1),
            "Test",
            "BK001",
            "Pending"
        );

        _controller.ControllerContext.HttpContext.Request.Headers["X-Login-Token"] = "valid-token";
        
        _mockTokenService.Setup(t => t.TryValidate("valid-token", out It.Ref<List<int>>.IsAny))
            .Callback(new TryValidateDelegate((string token, out List<int> ids) => 
            {
                ids = bookingIds;
            }))
            .Returns(true);
        
        _mockService.Setup(s => s.GetById(1)).ReturnsAsync(booking);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooking = Assert.IsType<BookingDto>(okResult.Value);
        Assert.Equal(1, returnedBooking.Id);
    }

    [Fact]
    public async Task GetById_WithValidTokenButUnauthorizedBooking_ReturnsForbidden()
    {
        // Arrange
        var bookingIds = new List<int> { 1 }; // User is authorized for booking 1

        _controller.ControllerContext.HttpContext.Request.Headers["X-Login-Token"] = "valid-token";
        
        _mockTokenService.Setup(t => t.TryValidate("valid-token", out It.Ref<List<int>>.IsAny))
            .Callback(new TryValidateDelegate((string token, out List<int> ids) => 
            {
                ids = bookingIds;
            }))
            .Returns(true);

        // Act - trying to access booking 999
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WithValidTokenAndAuthorizedBooking_ReturnsNoContent()
    {
        // Arrange
        var bookingIds = new List<int> { 1 };

        _controller.ControllerContext.HttpContext.Request.Headers["X-Login-Token"] = "valid-token";
        
        _mockTokenService.Setup(t => t.TryValidate("valid-token", out It.Ref<List<int>>.IsAny))
            .Callback(new TryValidateDelegate((string token, out List<int> ids) => 
            {
                ids = bookingIds;
            }))
            .Returns(true);
        
        _mockService.Setup(s => s.Delete(1)).ReturnsAsync((true, null));

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_BookingNotFound_ReturnsBadRequest()
    {
        // Arrange
        var bookingIds = new List<int> { 1 };

        _controller.ControllerContext.HttpContext.Request.Headers["X-Login-Token"] = "valid-token";
        
        _mockTokenService.Setup(t => t.TryValidate("valid-token", out It.Ref<List<int>>.IsAny))
            .Callback(new TryValidateDelegate((string token, out List<int> ids) => 
            {
                ids = bookingIds;
            }))
            .Returns(true);
        
        _mockService.Setup(s => s.Delete(1)).ReturnsAsync((false, "Booking not found."));

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    // Delegate for Moq callback
    private delegate void TryValidateDelegate(string token, out List<int> ids);
}
