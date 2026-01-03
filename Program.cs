using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VehicleHub.Api.Domain.DTOs;
using VehicleHub.Api.Domain.Entity;
using VehicleHub.Api.Domain.Enuns;
using VehicleHub.Api.Domain.Interfaces;
using VehicleHub.Api.Domain.ModelViews;
using VehicleHub.Api.Domain.Service;
using VehicleHub.Api.Infrastructure.Db;

#region Builder

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdminInterface, AdminServies>();
builder.Services.AddScoped<IVehicleInterface, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")
    );
});

var app = builder.Build();
#endregion

string GenerateJwtToken(Admin admin)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.Email, admin.Email),
        new Claim("Perfil", admin.Perfil),
    };

    var  token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
        );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminInterface adminServies) =>{

    var adm = adminServies.Login(loginDTO);
    if (adm != null)
    {
        string token = GenerateJwtToken(adm);
        return Results.Ok(new AdminLogged
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
   
}).WithTags("Admins");


app.MapGet("/admin", ([FromQuery] int? page, IAdminInterface adminServies) =>
{
    var adms = new List<AdminModelView>();
    var admins = adminServies.All(page);

    foreach (var adm in admins)
    {
        adms.Add(new AdminModelView
        {
            id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil

        });
    }

    return Results.Ok(adms);

}).RequireAuthorization().WithTags("Admins");

app.MapGet("/admins/{id}", ([FromRoute] int id, IAdminInterface adminServies) =>
{

    var admin = adminServies.SearchForId(id);

    if (adminServies == null) return Results.NotFound();

    return Results.Ok(new AdminModelView
    {
        id = admin.Id,
        Email = admin.Email,
        Perfil = admin.Perfil

    });

}).RequireAuthorization().WithTags("Admins");


app.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdminInterface adminServies) =>
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(adminDTO.Email))

        validation.Messages.Add("Email addresses cannot be empty.");

    if (string.IsNullOrEmpty(adminDTO.Senha))

        validation.Messages.Add("The password cannot be empty.");

    if (adminDTO.Perfil == null)

        validation.Messages.Add("The Profile field cannot be empty.");


    if (validation.Messages.Count > 0)

        return Results.BadRequest(validation);


    var admin = new Admin
    {
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Perfil = adminDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    adminServies.Save(admin);

    return Results.Created($"/admin/{admin.Id}", new AdminModelView
    {
        id = admin.Id,
        Email = admin.Email,
        Perfil = admin.Perfil

    });

}).RequireAuthorization().WithTags("Admins");

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
        Year = vehicleDTO.Year,
    };

    vehicleService.Save(vehicle);

    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);

}).RequireAuthorization().WithTags("vehicles");

app.MapGet("/vehicle", ([FromQuery] int? page, IVehicleInterface vehicleService) =>
{

    var vehicle = vehicleService.ALl(page);

    return Results.Ok(vehicle);

}).RequireAuthorization().WithTags("vehicles");


app.MapGet("/vehicle/{id}", ([FromRoute] int id, IVehicleInterface vehicleService) =>
{

    var vehicle = vehicleService.SearchForId(id);

    if (vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);

}).RequireAuthorization().WithTags("vehicles");



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

}).RequireAuthorization().WithTags("vehicles");



app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleInterface vehicleService) =>
{

    var vehicle = vehicleService.SearchForId(id);

    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);


    return Results.NoContent();

}).RequireAuthorization().WithTags("vehicles");




#endregion




app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();


app.Run();