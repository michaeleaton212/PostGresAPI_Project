﻿using Microsoft.EntityFrameworkCore;
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

// CORS für Angular-Dev (Origin ggf. auf https ändern, falls du https nutzt)
builder.Services.AddCors(options => // add CORS policy that allows requests from Angular server
{
    options.AddPolicy("NgDev", policy => // define policy named "NgDev"
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200" 
            )
            .AllowAnyHeader()// allow any header 
            .AllowAnyMethod() // allow any method (GET, POST, Update, Delete .)
    );
});

var app = builder.Build(); // build app

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseHttpsRedirection(); // use automaticly https redirection

app.UseRouting(); // use routing
app.UseCors("NgDev"); // aktivate CORS policy
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
