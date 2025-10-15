namespace MinimalApi.Infraestutura.Db;

using Microsoft.EntityFrameworkCore;
using  MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;

public class DbContexto : DbContext
{
    public DbContexto(DbContextOptions<DbContexto> options) : base(options)
    {
    }

    public DbSet<Administrador> Administradores { get; set; } = null!;
    public DbSet<Veiculo> Veiculos { get; set; } = null!;

    
}