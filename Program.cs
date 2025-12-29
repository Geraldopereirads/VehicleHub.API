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

app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminInterface adminServies) =>
{
    if (adminServies.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Admins");

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Vehicles
ValidationErrors validationDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name))

        validation.Messages.Add("The name cannot be empty.");


    if (string.IsNullOrEmpty(vehicleDTO.Mark))

        validation.Messages.Add("The mark cannot be empty.");


    if (vehicleDTO.Year < 1950)

        validation.Messages.Add("Very old vehicles, only those manufactured from 1950 onwards, will be accepted.");

    return validation;

}

app.MapPost("/vehicles", ([FromBody] VehicleDTO
    vehicleDTO, IVehicleInterface vehicleService) =>
{

    var validation = validationDTO(vehicleDTO);

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Mark = vehicleDTO.Mark,
        Year = vehicleDTO.Year
    };

    vehicleService.Save(vehicle);

    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);

}).WithTags("vehicles");

app.MapGet("/vehicle", ([FromQuery] int? page, IVehicleInterface vehicleService) =>
{

    var vehicle = vehicleService.ALl(page);

    return Results.Ok(vehicle);

}).WithTags("vehicles");


app.MapGet("/vehicle/{id}", ([FromRoute] int id, IVehicleInterface vehicleService) =>
{

    var vehicle = vehicleService.SearchForId(id);

    if (vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);

}).WithTags("vehicles");



app.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleInterface vehicleService) =>
{

    var validation = validationDTO(vehicleDTO);

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    var vehicle = vehicleService.SearchForId(id);

    if (vehicle == null) return Results.NotFound();

    vehicle.Name = vehicleDTO.Name;
    vehicle.Mark = vehicleDTO.Mark;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.Update(vehicle);


    return Results.Ok(vehicle);

}).WithTags("vehicles");



app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleInterface vehicleService) =>
{

    var vehicle = vehicleService.SearchForId(id);

    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);


    return Results.NoContent();

}).WithTags("vehicles");




#endregion




app.UseSwagger();
app.UseSwaggerUI();


app.Run();