using Microsoft.EntityFrameworkCore;
using PostGresAPI.Models;
namespace PostGresAPI.Data;
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    // DbSets
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Meetingroom> Meetingrooms => Set<Meetingroom>();
    public DbSet<Bedroom> Bedrooms => Set<Bedroom>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<User> Users => Set<User>();
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
          
            // Seed data for Meetingrooms
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
    
            // Seed data for Bedrooms
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
        
        // BookingNumber configuration
        booking.Property(b => b.BookingNumber)
            .HasMaxLength(50)
            .IsRequired();

        // Status configuration
        booking.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue(BookingStatus.Pending);

        // VORHER (FALSCH):
        //booking.HasOne<Room>()           // Keine Navigation Property angegeben
        //       .WithMany()                // Keine Navigation Property angegeben
        //       .HasForeignKey(b => b.RoomId)
        //       .OnDelete(DeleteBehavior.Cascade);

        // FK to Rooms 
        booking.HasOne(b => b.Room)
               .WithMany(r => r.Bookings)
               .HasForeignKey(b => b.RoomId)
               .OnDelete(DeleteBehavior.Cascade);
        // Index for Time range queries
        booking.HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime })
               .HasDatabaseName("ix_booking_room_time");
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
            .HasMaxLength(320); // RFC-typischer Wert, optional required
    }
}