using System.Globalization;
using System.Net;
using PostGresAPI.Models;

namespace PostGresAPI.Services
{
    public static class EmailTemplates
    {
        public static string BookingConfirmationHtml(Booking booking)
        {
            var culture = CultureInfo.GetCultureInfo("de-CH");
            var start = booking.StartTime.ToString("dd.MM.yyyy HH:mm", culture);
            var end = booking.EndTime.ToString("dd.MM.yyyy HH:mm", culture);

            var duration = booking.EndTime - booking.StartTime;
            var durationText = GetDurationText(duration);

            var roomType = GetRoomTypeText(booking.Room);
            var totalPrice = CalculateTotalPrice(booking, duration);

            var guestName = Escape(booking.User?.UserName ?? booking.Title ?? "Gast");
            var roomName = Escape(booking.Room?.Name ?? "N/A");
            var bookingNumber = Escape(booking.BookingNumber ?? "N/A");

            var checkInTime = booking.StartTime.ToString("HH:mm", culture);
            var checkOutTime = booking.EndTime.ToString("HH:mm", culture);

            var styles = EmailStyles.GetBookingConfirmationStyles();

            return $@"
<!doctype html>
<html lang=""de"">
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Buchungsbestätigung</title>
    {styles}
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <div class=""header-icon"">✓</div>
            <h1>Bestätigung</h1>
        </div>

        <div class=""content"">
            <p class=""greeting"">Hallo {guestName},</p>
            <p class=""intro"">
                Vielen Dank für Ihre Buchung. Ihr Platz im Digitalen Zeitalter ist reserviert. Hier sind Ihre Details:
            </p>

            <div class=""booking-card"">
                <span class=""booking-number-label"">Buchungsnummer</span>
                <div class=""booking-number"">{bookingNumber}</div>
            </div>

            <table class=""details-table"">
                <tr>
                    <td>Raum</td>
                    <td>{roomName}</td>
                </tr>
                <tr>
                    <td>Raumtyp</td>
                    <td>{roomType}</td>
                </tr>
                <tr>
                    <td>Check-in</td>
                    <td>{start} Uhr</td>
                </tr>
                <tr>
                    <td>Check-out</td>
                    <td>{end} Uhr</td>
                </tr>
                <tr>
                    <td>Personen</td>
                    <td>{booking.NumberOfPersons}</td>
                </tr>
                <tr>
                    <td>Dauer</td>
                    <td>{durationText}</td>
                </tr>
                <tr class=""price-row"">
                    <td>Gesamtbetrag</td>
                    <td>{totalPrice:F2} CHF</td>
                </tr>
            </table>

            <div class=""info-box"">
                <h3>Wichtige Hinweise</h3>
                <ul>
                    <li>Check-in ist ab {checkInTime} Uhr möglich</li>
                    <li>Bitte Check-out bis spätestens {checkOutTime} Uhr</li>
                    <li>Ihre Buchungsnummer dient als Referenz vor Ort</li>
                    <li>Kostenloses High-Speed WLAN inklusive, login mit name und Buchungsnummer</li>
                </ul>
            </div>
        </div>

        <div class=""footer"">
            <p><strong>Hotel Management System v26</strong></p>
            <p>Diese Nachricht wurde automatisch generiert.</p>
            <div class=""footer-copyright"">
                © 2026 Hotel Management System. Designed for iOS 26.
            </div>
        </div>
    </div>
</body>
</html>";
        }

        public static string BookingConfirmationText(Booking booking)
        {
            var start = booking.StartTime.ToString("dd.MM.yyyy HH:mm");
            var end = booking.EndTime.ToString("dd.MM.yyyy HH:mm");
            var duration = booking.EndTime - booking.StartTime;

            var guestName = booking.User?.UserName ?? booking.Title ?? "Gast";

            return $@"
BUCHUNGSBESTÄTIGUNG
----------------------------------------------
Hallo {guestName}, vielen Dank für Ihre Buchung!

Buchungsnummer: {booking.BookingNumber}
Raum: {booking.Room?.Name ?? "N/A"}
Check-in: {start}
Check-out: {end}
Gesamtpreis: {CalculateTotalPrice(booking, duration):F2} CHF

Ihr Hotel Team";
        }

        private static string GetRoomTypeText(Room? room)
        {
            if (room == null) return "Unbekannt";
            return room switch
            {
                Bedroom => "Schlafzimmer",
                Meetingroom => "Meetingraum",
                _ => "Standardraum"
            };
        }

        private static decimal CalculateTotalPrice(Booking booking, TimeSpan duration)
        {
            if (booking.Room == null) return 0;
            decimal pricePerUnit = booking.Room switch
            {
                Bedroom b => b.PricePerNight,
                Meetingroom => 100m,
                _ => 0
            };
            var units = Math.Max(1, (int)Math.Ceiling(duration.TotalDays));
            return pricePerUnit * units * booking.NumberOfPersons;
        }

        private static string GetDurationText(TimeSpan duration)
        {
            if (duration.TotalDays >= 1)
            {
                var days = (int)Math.Ceiling(duration.TotalDays);
                return days == 1 ? "1 Nacht" : $"{days} Nächte";
            }
            var hours = (int)Math.Ceiling(duration.TotalHours);
            return hours == 1 ? "1 Stunde" : $"{hours} Stunden";
        }

        private static string Escape(string value) => WebUtility.HtmlEncode(value);
    }
}