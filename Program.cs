using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleHub.Api.Domain.DTOs;
using VehicleHub.Api.Domain.Service;
using VehicleHub.Api.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<IAdminInterface, AdminServies>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sqlserver")
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapPost("/login", ([FromBody] LoginDTO
    loginDTO, IAdminInterface adminServices) =>
{
    if (adminServices.Login(loginDTO) != null)

        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
});


app.Run();