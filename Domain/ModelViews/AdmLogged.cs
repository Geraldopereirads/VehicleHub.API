namespace VehicleHub.Api.Domain.ModelViews;

public record AdminLogged
{
    public string Email { get; set; } = default!;

    public string Perfil { get; set; } = default!;

    public string Token { get; set; } = default!;
}
