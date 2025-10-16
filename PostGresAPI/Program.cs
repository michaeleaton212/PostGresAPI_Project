using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Repository;
using PostGresAPI.Services;

var builder = WebApplication.CreateBuilder(args); // create builder

builder.Services.AddDbContext<ApplicationDbContext>(opt =>          //connect to PostgreSQL
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// dependency injection
builder.Services.AddScoped<UserRepository>(); // tells container to create a new instance of UserRepository for each request
builder.Services.AddScoped<RoomRepository>();
builder.Services.AddScoped<MeetingroomRepository>();
builder.Services.AddScoped<BedroomRepository>();
builder.Services.AddScoped<BookingRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<MeetingroomService>();
builder.Services.AddScoped<BedroomService>();
builder.Services.AddScoped<BookingService>();

builder.Services.AddControllers(); // add controllers
builder.Services.AddEndpointsApiExplorer(); // add swagger
builder.Services.AddSwaggerGen(); // add swagger

var app = builder.Build(); // build app
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();// use https redirection
app.MapControllers();
app.Run();
