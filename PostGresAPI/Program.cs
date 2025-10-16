using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Repository;
using PostGresAPI.Services;

var builder = WebApplication.CreateBuilder(args); // create builder

builder.Services.AddDbContext<ApplicationDbContext>(opt =>          //connect to PostgreSQL
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// dependency injection

// Dependency Injection - Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>(); // tells container to create a new instance of UserRepository for each request

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IMeetingroomRepository, MeetingroomRepository>();
builder.Services.AddScoped<IBedroomRepository, BedroomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Dependency Injection - Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IMeetingroomService, MeetingroomService>();
builder.Services.AddScoped<IBedroomService, BedroomService>();
builder.Services.AddScoped<IBookingService, BookingService>(); ;

builder.Services.AddControllers(); // add controllers
builder.Services.AddEndpointsApiExplorer(); // add swagger
builder.Services.AddSwaggerGen(); // add swagger

var app = builder.Build(); // build app
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();// use https redirection
app.MapControllers();
app.Run();
