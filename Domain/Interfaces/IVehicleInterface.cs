using VehicleHub.Api.Domain.Entity;

namespace VehicleHub.Api.Domain.Interfaces;

public interface IVehicleInterface
{
    List<Vehicle> ALl(int? page = 1, string? name = null, string? mark = null);

    Vehicle? SearchForId(int id);

    void Save(Vehicle vehicle);

    void Update(Vehicle vehicle);

    void Delete(Vehicle vehicle);
}
