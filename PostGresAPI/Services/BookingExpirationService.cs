using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PostGresAPI.Models;
using PostGresAPI.Repository;

namespace PostGresAPI.Services;

public class BookingExpirationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingExpirationService> _logger;
    private readonly TimeSpan _checkInterval;

    public BookingExpirationService(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingExpirationService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        
        // Lese Intervall aus appsettings.json, Standard: 1 Stunde
        var intervalHours = configuration.GetValue<double>("BookingExpiration:CheckIntervalHours", 1.0);
        _checkInterval = TimeSpan.FromHours(intervalHours);
        
        _logger.LogInformation($"BookingExpirationService initialized with check interval: {_checkInterval.TotalHours} hours");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BookingExpirationService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndExpireBookings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking expired bookings.");
            }

            // Warte bis zum nächsten Check
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("BookingExpirationService stopped.");
    }

    private async Task CheckAndExpireBookings()
    {
        using var scope = _scopeFactory.CreateScope();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

        var now = DateTimeOffset.UtcNow;

        _logger.LogDebug($"Checking for expired bookings at {now:yyyy-MM-dd HH:mm:ss} UTC");

        // Hole alle Buchungen, die abgelaufen sind aber noch nicht den Status "Expired" oder "Cancelled" haben
        var allBookings = await bookingRepository.GetAll();
        
        var expiredBookings = allBookings
            .Where(b => b.EndTime < now 
                     && b.Status != BookingStatus.Expired 
                     && b.Status != BookingStatus.Cancelled)
            .ToList();

        if (expiredBookings.Any())
        {
            _logger.LogInformation($"Found {expiredBookings.Count} expired booking(s). Setting status to Expired...");

            foreach (var booking in expiredBookings)
            {
                _logger.LogInformation($"  - Booking {booking.Id} ('{booking.Title}', EndTime: {booking.EndTime:yyyy-MM-dd HH:mm:ss}) -> Expired");
                await bookingRepository.UpdateStatus(booking.Id, BookingStatus.Expired);
            }

            _logger.LogInformation($"Successfully updated {expiredBookings.Count} booking(s) to Expired status.");
        }
        else
        {
            _logger.LogDebug("No expired bookings found.");
        }
    }
}
