using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleHub.Api.Domain.DTOs;
using VehicleHub.Api.Domain.ModelViews;
using VehicleHub.Api.Domain.Service;
using VehicleHub.Api.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<IAdminInterface, AdminServies>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sqlserver")
    );
});

var app = builder.Build();


app.MapGet("/", () => Results.Json(new Home()));


app.MapPost("/login", ([FromBody] LoginDTO
    loginDTO, IAdminInterface adminServices) =>
{
    if (adminServices.Login(loginDTO) != null)

        return Results.Ok("Login successful");
    else
        return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI();


app.Run();