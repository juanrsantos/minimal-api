using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Infraestutura.Db;

public class DbContexto : DbContext
{
    public DbContexto(DbContextOptions<DbContexto> options) : base(options)
    {
    }

    // Exemplo de DbSet - substitua por suas entidades reais
    public DbSet<object> Exemplo { get; set; } = null!;
    
    
}