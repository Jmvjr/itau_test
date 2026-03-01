using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CompraProgramada.Infrastructure.Data;

/// <summary>
/// Factory para criar DbContext em tempo de design (migrations, scaffolding)
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Usar connection string para design-time (local MySQL)
        var connectionString = "Server=localhost;Port=3306;Database=compra_programada;User=root;Password=root_password;";
        
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        
        return new AppDbContext(optionsBuilder.Options);
    }
}
