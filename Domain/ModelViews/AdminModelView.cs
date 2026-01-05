namespace VehicleHub.Api.Domain.ModelViews;

public record AdminModelView
{
    public int id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;

}
