using Microsoft.EntityFrameworkCore;
using VehicleHub.Api.Domain.Entity;

namespace VehicleHub.Api.Infrastructure.Db;

public class DbContexto : DbContext
{


    public DbContexto(DbContextOptions<DbContexto> options)
        : base(options)
    {
    }
    public DbSet<Admin> Admins { get; set; } = default!;
    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>().HasData(
            new Admin
            {
                Id = -1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }

}
