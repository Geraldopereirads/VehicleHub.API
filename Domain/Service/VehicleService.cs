using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using VehicleHub.Api.Domain.Entity;
using VehicleHub.Api.Domain.Interfaces;
using VehicleHub.Api.Infrastructure.Db;

namespace VehicleHub.Api.Domain.Service;

public class VehicleService : IVehicleInterface
{
    private readonly DbContexto _contexto;

    public VehicleService(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public List<Vehicle> ALl(int? page = 1, string? name = null, string? mark = null)
    {
        var query = _contexto.Vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name.ToLower()}%"));
        }


        int ItemsPerPage = 10;
        if (page != null)
        {
            query = query.Skip(((int)page - 1) * ItemsPerPage).Take(ItemsPerPage);
        }
        
        return query.ToList();
    }

    public void Delete(Vehicle vehicle)
    {
        _contexto.Vehicles.Remove(vehicle);
        _contexto.SaveChanges();
    }

    public void Save(Vehicle vehicle)
    {
        _contexto.Vehicles.Add(vehicle);
        _contexto.SaveChanges();
    }

    public Vehicle? SearchForId(int id)
    {
        return _contexto.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Update(Vehicle vehicle)
    {
        _contexto.Vehicles.Update(vehicle);
        _contexto.SaveChanges();
    }
}
