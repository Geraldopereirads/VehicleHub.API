using VehicleHub.Api.Domain.DTOs;
using VehicleHub.Api.Domain.Entity;
using VehicleHub.Api.Infrastructure.Db;

namespace VehicleHub.Api.Domain.Service;


public class AdminServies : IAdminServices
{

    private readonly DbContexto _contexto;

    public AdminServies(DbContexto contexto)
    {
        _contexto = contexto;
    }
    public Admin? Login(LoginDTO loginDTO)
    {

        var adm = _contexto.Admins.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();

        return adm;


    }       
    
}
