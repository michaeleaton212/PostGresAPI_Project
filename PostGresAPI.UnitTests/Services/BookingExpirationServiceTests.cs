using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PostGresAPI.Models;
using PostGresAPI.Repository;
using PostGresAPI.Services;

namespace PostGresAPI.UnitTests.Services;

public class BookingExpirationServiceTests
{
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly Mock<IServiceScope> _mockScope;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IBookingRepository> _mockRepo;
    private readonly Mock<ILogger<BookingExpirationService>> _mockLogger;
    private readonly IConfiguration _configuration;

    public BookingExpirationServiceTests()
    {
        _mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockScope = new Mock<IServiceScope>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockRepo = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingExpirationService>>();

        // Setup configuration
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "BookingExpiration:CheckIntervalHours", "0.0001" } // Very short interval for testing (0.36 seconds)
        });
        _configuration = configBuilder.Build();

        // Setup service scope chain
        _mockScopeFactory
            .Setup(f => f.CreateScope())
            .Returns(_mockScope.Object);

        _mockScope
            .Setup(s => s.ServiceProvider)
            .Returns(_mockServiceProvider.Object);

        _mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IBookingRepository)))
            .Returns(_mockRepo.Object);
    }

    [Fact]
    public void Constructor_InitializesWithDefaultInterval_WhenNotConfigured()
    {
        // Arrange
        var emptyConfigBuilder = new ConfigurationBuilder();
        emptyConfigBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var emptyConfig = emptyConfigBuilder.Build();

        // Act
        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, emptyConfig);

        // Assert
        Assert.NotNull(service);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("1 hours")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_InitializesWithCustomInterval_WhenConfigured()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "BookingExpiration:CheckIntervalHours", "2.5" }
        });
        var config = configBuilder.Build();

        // Act
        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, config);

        // Assert
        Assert.NotNull(service);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("2.5 hours")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_LogsStartMessage()
    {
        // Arrange
        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();
        
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(new List<Booking>());

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(100); // Give it time to start
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("BookingExpirationService started")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesExpiredBookings()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var expiredBooking = new Booking(1, now.AddHours(-2), now.AddHours(-1), "Expired Booking", "BK001");
        // Set Id using reflection since it's private set
        typeof(Booking).GetProperty("Id")!.SetValue(expiredBooking, 1);
        
        var bookings = new List<Booking> { expiredBooking };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bookings);
        _mockRepo.Setup(r => r.UpdateStatus(It.IsAny<int>(), BookingStatus.Expired))
            .ReturnsAsync(expiredBooking);

        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(500); // Wait for at least one check cycle
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce);
        _mockRepo.Verify(r => r.UpdateStatus(1, BookingStatus.Expired), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotUpdateActiveBookings()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var activeBooking = new Booking(1, now.AddHours(-1), now.AddHours(1), "Active Booking", "BK001");
        typeof(Booking).GetProperty("Id")!.SetValue(activeBooking, 1);
        
        var bookings = new List<Booking> { activeBooking };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bookings);

        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(500);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce);
        _mockRepo.Verify(r => r.UpdateStatus(It.IsAny<int>(), It.IsAny<BookingStatus>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotUpdateAlreadyExpiredBookings()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var alreadyExpiredBooking = new Booking(1, now.AddHours(-2), now.AddHours(-1), "Already Expired", "BK001");
        typeof(Booking).GetProperty("Id")!.SetValue(alreadyExpiredBooking, 1);
        
        // Use reflection to set internal Status property
        typeof(Booking).GetProperty("Status")!.SetValue(alreadyExpiredBooking, BookingStatus.Expired);
        
        var bookings = new List<Booking> { alreadyExpiredBooking };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bookings);

        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(500);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce);
        _mockRepo.Verify(r => r.UpdateStatus(It.IsAny<int>(), It.IsAny<BookingStatus>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotUpdateCancelledBookings()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var cancelledBooking = new Booking(1, now.AddHours(-2), now.AddHours(-1), "Cancelled Booking", "BK001");
        typeof(Booking).GetProperty("Id")!.SetValue(cancelledBooking, 1);
        
        // Use reflection to set internal Status property
        typeof(Booking).GetProperty("Status")!.SetValue(cancelledBooking, BookingStatus.Cancelled);
        
        var bookings = new List<Booking> { cancelledBooking };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bookings);

        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(500);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.GetAll(), Times.AtLeastOnce);
        _mockRepo.Verify(r => r.UpdateStatus(It.IsAny<int>(), It.IsAny<BookingStatus>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesMultipleExpiredBookings()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var expiredBooking1 = new Booking(1, now.AddHours(-3), now.AddHours(-2), "Expired 1", "BK001");
        typeof(Booking).GetProperty("Id")!.SetValue(expiredBooking1, 1);
        
        var expiredBooking2 = new Booking(2, now.AddHours(-2), now.AddHours(-1), "Expired 2", "BK002");
        typeof(Booking).GetProperty("Id")!.SetValue(expiredBooking2, 2);
        
        var bookings = new List<Booking> { expiredBooking1, expiredBooking2 };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bookings);
        _mockRepo.Setup(r => r.UpdateStatus(It.IsAny<int>(), BookingStatus.Expired))
            .ReturnsAsync((int id, BookingStatus status) => bookings.First(b => b.Id == id));

        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(500);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.UpdateStatus(1, BookingStatus.Expired), Times.AtLeastOnce);
        _mockRepo.Verify(r => r.UpdateStatus(2, BookingStatus.Expired), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesRepositoryExceptions()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAll()).ThrowsAsync(new Exception("Database error"));

        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(500);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert - should log error but not crash
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error occurred while checking expired bookings")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_LogsFoundExpiredBookings()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var expiredBooking = new Booking(1, now.AddHours(-2), now.AddHours(-1), "Expired Booking", "BK001");
        typeof(Booking).GetProperty("Id")!.SetValue(expiredBooking, 1);
        
        var bookings = new List<Booking> { expiredBooking };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bookings);
        _mockRepo.Setup(r => r.UpdateStatus(It.IsAny<int>(), BookingStatus.Expired))
            .ReturnsAsync(expiredBooking);

        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(500);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Found 1 expired booking")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesOnlyPendingOrCheckedInExpiredBookings()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        
        // Pending and expired -> should update (Pending is default)
        var pendingExpired = new Booking(1, now.AddHours(-2), now.AddHours(-1), "Pending Expired", "BK001");
        typeof(Booking).GetProperty("Id")!.SetValue(pendingExpired, 1);
        
        // CheckedIn and expired -> should update
        var checkedInExpired = new Booking(2, now.AddHours(-2), now.AddHours(-1), "CheckedIn Expired", "BK002");
        typeof(Booking).GetProperty("Id")!.SetValue(checkedInExpired, 2);
        typeof(Booking).GetProperty("Status")!.SetValue(checkedInExpired, BookingStatus.CheckedIn);
        
        var bookings = new List<Booking> { pendingExpired, checkedInExpired };
        _mockRepo.Setup(r => r.GetAll()).ReturnsAsync(bookings);
        _mockRepo.Setup(r => r.UpdateStatus(It.IsAny<int>(), BookingStatus.Expired))
            .ReturnsAsync((int id, BookingStatus status) => bookings.First(b => b.Id == id));

        var service = new BookingExpirationService(_mockScopeFactory.Object, _mockLogger.Object, _configuration);
        var cts = new CancellationTokenSource();

        // Act
        var executeTask = service.StartAsync(cts.Token);
        await Task.Delay(500);
        cts.Cancel();
        await service.StopAsync(CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.UpdateStatus(1, BookingStatus.Expired), Times.AtLeastOnce);
        _mockRepo.Verify(r => r.UpdateStatus(2, BookingStatus.Expired), Times.AtLeastOnce);
    }
}
