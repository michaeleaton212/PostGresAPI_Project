// ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using PostGresAPI.Models;
using PostGresAPI.Persistence.Entities;

namespace PostGresAPI.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Meetingroom> Meetingrooms => Set<Meetingroom>();
    public DbSet<Bedroom> Bedrooms => Set<Bedroom>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Review> Reviews => Set<Review>();

    // NEU: Outbox (für E-Mails)
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ROOM
        var room = modelBuilder.Entity<Room>();
        room.ToTable("rooms");
        room.HasKey(r => r.Id);
        room.Property(r => r.Id)
            .ValueGeneratedOnAdd();
        room.Property(r => r.Name)
            .HasMaxLength(200)
            .IsRequired();
        room.Property(r => r.ImagePath)
            .HasColumnName("image");

        // TPH-Discriminator
        room.HasDiscriminator<string>("room_type")
            .HasValue<Meetingroom>("Meetingroom")
            .HasValue<Bedroom>("Bedroom");

        // Meetingroom-specific columns
        modelBuilder.Entity<Meetingroom>(m =>
        {
            m.Property(x => x.NumberOfChairs)
             .HasColumnName("number_of_chairs")
             .HasDefaultValue(0);

            m.HasData(
                new { Id = 1, Name = "Conference Room A", NumberOfChairs = 20 },
                new { Id = 2, Name = "Conference Room B", NumberOfChairs = 15 },
                new { Id = 3, Name = "Board Room", NumberOfChairs = 10 }
            );
        });

        // Bedroom-specific columns
        modelBuilder.Entity<Bedroom>(b =>
        {
            b.Property(x => x.NumberOfBeds)
             .HasColumnName("number_of_beds")
             .HasDefaultValue(0);

            b.Property(x => x.PricePerNight)
             .HasColumnName("price_per_night")
             .HasColumnType("decimal(18,2)")
             .IsRequired()
             .HasDefaultValue(0m);

            b.HasData(
                new { Id = 4, Name = "Room 101", NumberOfBeds = 1, PricePerNight = 50m },
                new { Id = 5, Name = "Room 102", NumberOfBeds = 2, PricePerNight = 80m },
                new { Id = 6, Name = "Room 103", NumberOfBeds = 2, PricePerNight = 80m },
                new { Id = 7, Name = "Suite 201", NumberOfBeds = 3, PricePerNight = 120m }
            );
        });

        // BOOKING
        var booking = modelBuilder.Entity<Booking>();
        booking.ToTable("bookings");
        booking.HasKey(b => b.Id);
        booking.Property(b => b.Id)
               .ValueGeneratedOnAdd();
        booking.Property(b => b.Title)
               .HasMaxLength(200);
        booking.Property(b => b.StartTime)
               .IsRequired();
        booking.Property(b => b.EndTime)
               .IsRequired();
        booking.Property(b => b.RoomId)
               .IsRequired();

        booking.Property(b => b.BookingNumber)
            .HasMaxLength(50)
            .IsRequired();

        booking.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue(BookingStatus.Pending);

        booking.Property(b => b.NumberOfPersons)
            .IsRequired()
            .HasDefaultValue(1);

        // FK to Rooms
        booking.HasOne(b => b.Room)
               .WithMany(r => r.Bookings)
               .HasForeignKey(b => b.RoomId)
               .OnDelete(DeleteBehavior.Cascade);

        // FK to Users (optional)
        booking.HasOne(b => b.User)
               .WithMany()
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        booking.HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime })
               .HasDatabaseName("ix_booking_room_time");

        booking.HasIndex(b => b.UserId)
               .HasDatabaseName("ix_booking_user");

        // USER
        var user = modelBuilder.Entity<User>();
        user.ToTable("Users");
        user.HasKey(u => u.Id);
        user.Property(u => u.Id)
            .ValueGeneratedOnAdd();
        user.Property(u => u.UserName)
            .HasMaxLength(200)
            .IsRequired();
        user.Property(u => u.Email)
            .HasMaxLength(320);

        // REVIEW
        var review = modelBuilder.Entity<Review>();
        review.ToTable("Reviews");
        review.HasKey(r => r.Id);
        review.Property(r => r.Id)
            .ValueGeneratedOnAdd();
        review.Property(r => r.Title)
            .HasMaxLength(200)
            .IsRequired();
        review.Property(r => r.Content)
            .IsRequired();
        review.Property(r => r.Rating)
            .IsRequired();
        review.Property(r => r.CreatedAt)
            .IsRequired();
        review.Property(r => r.UpdatedAt);

        review.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        review.HasIndex(r => r.UserId);
        review.HasIndex(r => r.CreatedAt);
        review.HasIndex(r => r.Rating);

        // NEU: OUTBOX EMAIL
        var outbox = modelBuilder.Entity<OutboxEmail>();
        outbox.ToTable("outbox_emails");
        outbox.HasKey(x => x.Id);

        outbox.Property(x => x.Id)
              .ValueGeneratedOnAdd();

        outbox.Property(x => x.CreatedUtc)
              .IsRequired();

        outbox.Property(x => x.To)
              .HasMaxLength(320)
              .IsRequired();

        outbox.Property(x => x.Subject)
              .HasMaxLength(300)
              .IsRequired();

        outbox.Property(x => x.HtmlBody)
              .IsRequired();

        outbox.Property(x => x.TextBody)
              .IsRequired();

        outbox.Property(x => x.SentUtc);

        outbox.Property(x => x.TryCount)
              .IsRequired()
              .HasDefaultValue(0);

        outbox.Property(x => x.LastError)
              .HasMaxLength(2000);

        outbox.HasIndex(x => x.SentUtc)
              .HasDatabaseName("ix_outbox_sentutc");

        outbox.HasIndex(x => new { x.SentUtc, x.CreatedUtc })
              .HasDatabaseName("ix_outbox_pending_order");
    }
}
