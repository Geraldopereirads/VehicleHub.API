using Microsoft.EntityFrameworkCore;
using VehicleHub.Api.Domain.DTOs;
using VehicleHub.Api.Domain.Entity;
using VehicleHub.Api.Infrastructure.Db;

namespace VehicleHub.Api.Domain.Service;


public class AdminServies : IAdminInterface
{

    private readonly DbContexto _contexto;

    public AdminServies(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public List<Admin> All(int? page)
    {
        var query = _contexto.Admins.AsQueryable();
        
        int ItemsPerPage = 10;
        if (page != null)
        {
            query = query.Skip(((int)page - 1) * ItemsPerPage).Take(ItemsPerPage);
        }

        return query.ToList();
    }

    public Admin? Login(LoginDTO loginDTO)
    {

        var adm = _contexto.Admins.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();

        return adm;


    }

    public Admin Save(Admin admin)
    {
        _contexto.Admins.Add(admin);
        _contexto.SaveChanges();

        return admin;
    }
}
