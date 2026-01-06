using System.Globalization;
using System.Net;
using PostGresAPI.Models;

namespace PostGresAPI.Services
{
    public static class EmailTemplates
    {
        public static string BookingConfirmationHtml(Booking booking)
        {
            var start = booking.StartTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("de-CH"));
            var end = booking.EndTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("de-CH"));

            var duration = booking.EndTime - booking.StartTime;
            var durationText = GetDurationText(duration);

            var roomType = GetRoomTypeText(booking.Room);
            var totalPrice = CalculateTotalPrice(booking, duration);

            var guestName = Escape(booking.Title ?? "Gast");
            var roomName = Escape(booking.Room?.Name ?? "N/A");
            var bookingNumber = Escape(booking.BookingNumber ?? "");

            var checkInTime = booking.StartTime.ToString("HH:mm", CultureInfo.GetCultureInfo("de-CH"));
            var checkOutTime = booking.EndTime.ToString("HH:mm", CultureInfo.GetCultureInfo("de-CH"));

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
      <div class=""header-icon"">?</div>
      <h1>Buchungsbestätigung</h1>
    </div>
    
    <div class=""content"">
      <p class=""greeting"">Hallo {guestName},</p>
      <p class=""intro"">Vielen Dank für Ihre Buchung! Wir freuen uns, Sie bei uns begrüßen zu dürfen.</p>
      
      <div class=""booking-number-section"">
        <span class=""booking-number-label"">Ihre Buchungsnummer</span>
        <div class=""booking-number"">{bookingNumber}</div>
      </div>

      <table class=""details-table"">
        <tr>
          <td><span class=""icon"">??</span>Raum</td>
          <td>{roomName}</td>
        </tr>
        <tr>
          <td><span class=""icon"">???</span>Raumtyp</td>
          <td>{roomType}</td>
        </tr>
        <tr>
          <td><span class=""icon"">??</span>Check-in</td>
          <td>{start} Uhr</td>
        </tr>
        <tr>
          <td><span class=""icon"">??</span>Check-out</td>
          <td>{end} Uhr</td>
        </tr>
        <tr>
          <td><span class=""icon"">??</span>Anzahl Personen</td>
          <td>{booking.NumberOfPersons} {(booking.NumberOfPersons == 1 ? "Person" : "Personen")}</td>
        </tr>
        <tr>
          <td><span class=""icon"">??</span>Aufenthaltsdauer</td>
          <td>{durationText}</td>
        </tr>
        <tr class=""price-row"">
          <td><span class=""icon"">??</span>Gesamtpreis</td>
          <td>{totalPrice:F2} CHF</td>
        </tr>
      </table>

      <div class=""info-box"">
        <h3>Wichtige Hinweise</h3>
        <ul>
          <li>Bitte bewahren Sie Ihre Buchungsnummer auf</li>
          <li>Check-in ist ab {checkInTime} Uhr möglich</li>
          <li>Check-out bis {checkOutTime} Uhr</li>
          <li>Bei Fragen stehen wir Ihnen gerne zur Verfügung</li>
        </ul>
      </div>
    </div>

    <div class=""footer"">
      <p><strong>Hotel Management System</strong></p>
      <p>Diese E-Mail wurde automatisch erstellt.</p>
      <p>Bei Fragen kontaktieren Sie uns bitte.</p>
      <div class=""footer-copyright"">
        © 2026 Hotel Management System. Alle Rechte vorbehalten.
      </div>
    </div>
  </div>
</body>
</html>";
        }

        public static string BookingConfirmationText(Booking booking)
        {
            var start = booking.StartTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("de-CH"));
            var end = booking.EndTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("de-CH"));
            
            var duration = booking.EndTime - booking.StartTime;
            var durationText = GetDurationText(duration);
            
            var roomType = GetRoomTypeText(booking.Room);
            var totalPrice = CalculateTotalPrice(booking, duration);

            return $@"
**********************************************
         BUCHUNGSBESTÄTIGUNG
**********************************************

Hallo {booking.Title ?? "Gast"},

Vielen Dank fuer Ihre Buchung!

----------------------------------------------
BUCHUNGSDETAILS
----------------------------------------------

Buchungsnummer:     {booking.BookingNumber}
Raum:               {booking.Room?.Name ?? "N/A"}
Raumtyp:            {roomType}
Check-in:           {start}
Check-out:          {end}
Anzahl Personen:    {booking.NumberOfPersons}
Aufenthaltsdauer:   {durationText}

----------------------------------------------
PREIS
----------------------------------------------

Gesamtpreis:        {totalPrice:F2} CHF

----------------------------------------------

WICHTIGE HINWEISE:
- Bewahren Sie Ihre Buchungsnummer auf
- Check-in ab {booking.StartTime.ToString("HH:mm", CultureInfo.GetCultureInfo("de-CH"))} Uhr
- Check-out bis {booking.EndTime.ToString("HH:mm", CultureInfo.GetCultureInfo("de-CH"))} Uhr

Bei Fragen kontaktieren Sie uns bitte.

Diese E-Mail wurde automatisch erstellt.

(c) 2026 Hotel Management System
**********************************************
";
        }

        private static string GetRoomTypeText(Room? room)
        {
            if (room == null) return "Unbekannt";

            return room switch
            {
                Bedroom => "Schlafzimmer",
                Meetingroom => "Meetingraum",
                _ => "Raum"
            };
        }

        private static decimal CalculateTotalPrice(Booking booking, TimeSpan duration)
        {
            if (booking.Room == null) return 0;

            decimal pricePerNight = booking.Room switch
            {
                Bedroom b => b.PricePerNight,
                Meetingroom => 100m,
                _ => 0
            };

            var nights = Math.Max(1, (int)Math.Ceiling(duration.TotalDays));
            return pricePerNight * nights * booking.NumberOfPersons;
        }

        private static string GetDurationText(TimeSpan duration)
        {
            if (duration.TotalDays >= 1)
            {
                var days = (int)Math.Ceiling(duration.TotalDays);
                return days == 1 ? "1 Nacht" : $"{days} Naechte";
            }

            var hours = (int)Math.Ceiling(duration.TotalHours);
            return hours == 1 ? "1 Stunde" : $"{hours} Stunden";
        }

        private static string Escape(string value)
            => WebUtility.HtmlEncode(value);
    }
}
