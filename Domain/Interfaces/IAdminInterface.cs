using VehicleHub.Api.Domain.DTOs;
using VehicleHub.Api.Domain.Entity;

namespace VehicleHub.Api.Domain.Service;

public interface IAdminInterface
{
    Admin? Login(LoginDTO loginDTO);
}
