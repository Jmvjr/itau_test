using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Infrastructure.Data;
using CompraProgramada.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CompraProgramada.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Registrar DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mysqlOptions => mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null))
        );

        // Registrar Repositories
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IRepository<ContaGrafica>, ContaGraficaRepository>();
        services.AddScoped<IRepository<Custodia>, CustodiaRepository>();
        services.AddScoped<IRepository<Distribuicao>, DistribuicaoRepository>();
        services.AddScoped<IRepository<CestaRecomendacao>, CestaRepository>();
        services.AddScoped<IRepository<OrdemCompra>, OrdemCompraRepository>();
        services.AddScoped<IRepository<Cotacao>, CotacaoRepository>();
        services.AddScoped<IRepository<EventoIR>, EventoIRRepository>();
        services.AddScoped<IRepository<Rebalanceamento>, RebalanceamentoRepository>();

        return services;
    }
}
