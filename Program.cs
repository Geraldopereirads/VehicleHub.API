using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleHub.Api.Domain.DTOs;
using VehicleHub.Api.Domain.Entity;
using VehicleHub.Api.Domain.Interfaces;
using VehicleHub.Api.Domain.ModelViews;
using VehicleHub.Api.Domain.Service;
using VehicleHub.Api.Infrastructure.Db;

#region Builder

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<IAdminInterface, AdminServies>();
builder.Services.AddScoped<IVehicleInterface, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sqlserver")
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Vehicles
app.MapPost("/vehicles", ([FromBody] VehicleDTO
    vehicleDTO, IVehicleInterface vehicleService) => {

        var vehicle = new Vehicle
        {
            Name = vehicleDTO.Name,
            Mark = vehicleDTO.Mark,
            Year = vehicleDTO.Year
        };

        vehicleService.Save(vehicle);

        return Results.Created($"/vehicle/{vehicle.Id}", vehicle);

});
#endregion




app.UseSwagger();
app.UseSwaggerUI();


app.Run();