using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Enums;
using CompraProgramada.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CompraProgramada.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<ContaGrafica> ContasGraficas { get; set; } = null!;
    public DbSet<Custodia> Custodias { get; set; } = null!;
    public DbSet<Distribuicao> Distribuicoes { get; set; } = null!;
    public DbSet<CestaRecomendacao> CestasRecomendadas { get; set; } = null!;
    public DbSet<OrdemCompra> OrdensCompra { get; set; } = null!;
    public DbSet<Cotacao> Cotacoes { get; set; } = null!;
    public DbSet<EventoIR> EventosIR { get; set; } = null!;
    public DbSet<Rebalanceamento> Rebalanceamentos { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCliente(modelBuilder);
        ConfigureContaGrafica(modelBuilder);
        ConfigureCustodia(modelBuilder);
        ConfigureDistribuicao(modelBuilder);
        ConfigureCestaRecomendacao(modelBuilder);
        ConfigureOrdemCompra(modelBuilder);
        ConfigureCotacao(modelBuilder);
        ConfigureEventoIR(modelBuilder);
        ConfigureRebalanceamento(modelBuilder);
    }

    private void ConfigureCliente(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Cliente>();

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.ValorMensal)
            .HasColumnType("DECIMAL(18,2)")
            .IsRequired();

        // CPF como ValueObject mapeado para coluna VARCHAR
        builder.Property(c => c.CPF)
            .HasConversion(
                cpf => cpf.Valor,
                valor => new CPF(valor))
            .HasColumnName("CPF")
            .HasMaxLength(11)
            .IsRequired();
        
        builder.HasIndex(c => c.CPF).IsUnique().HasDatabaseName("UX_Cliente_CPF");

        builder.Property(c => c.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.DataAdesao)
            .HasColumnType("DATETIME")
            .IsRequired();

        // Relacionamento: Cliente 1:1 ContaGrafica (Filhote)
        builder.HasOne<ContaGrafica>()
            .WithOne(cg => cg.Cliente)
            .HasForeignKey<ContaGrafica>(cg => cg.ClienteId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }

    private void ConfigureContaGrafica(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<ContaGrafica>();

        builder.HasKey(cg => cg.Id);
        builder.Property(cg => cg.Id).ValueGeneratedOnAdd();

        builder.Property(cg => cg.ClienteId);

        builder.Property(cg => cg.NumeroConta)
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(cg => cg.NumeroConta).IsUnique().HasDatabaseName("UX_ContaGrafica_Numero");

        builder.Property(cg => cg.Tipo)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(cg => cg.DataCriacao)
            .HasColumnType("DATETIME")
            .IsRequired();

        // Relacionamento: ContaGrafica 1:N Custodia
        builder.HasMany<Custodia>()
            .WithOne()
            .HasForeignKey("ContaGraficaId")
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureCustodia(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Custodia>();

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.ContaGraficaId)
            .IsRequired();

        // Ticker como ValueObject mapeado para coluna VARCHAR
        builder.Property(c => c.Ticker)
            .HasConversion(
                ticker => ticker.Valor,
                valor => new Ticker(valor))
            .HasColumnName("Ticker")
            .HasMaxLength(10)
            .IsRequired();

        // Quantidade como ValueObject mapeado para coluna INT
        builder.Property(c => c.Quantidade)
            .HasConversion(
                qtd => qtd.Valor,
                valor => new Quantidade(valor))
            .HasColumnName("Quantidade")
            .IsRequired();

        builder.Property(c => c.PrecoMedio)
            .HasColumnType("DECIMAL(18,4)")
            .IsRequired();

        builder.Property(c => c.DataUltimaAtualizacao)
            .HasColumnType("DATETIME")
            .IsRequired();

        // Índice composto para consultas rápidas
        builder.HasIndex(c => new { c.ContaGraficaId, c.Ticker }).IsUnique().HasDatabaseName("UX_Custodia_ContaTicket");
    }

    private void ConfigureDistribuicao(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Distribuicao>();

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedOnAdd();

        builder.Property(d => d.OrdemCompraId)
            .IsRequired(false); // Nullable para compras programadas

        builder.Property(d => d.ClienteId)
            .IsRequired();

        builder.Property(d => d.ContaMasterId)
            .IsRequired();

        // Ticker como ValueObject
        builder.Property(d => d.Ticker)
            .HasConversion(
                ticker => ticker.Valor,
                valor => new Ticker(valor))
            .HasColumnName("Ticker")
            .HasMaxLength(10)
            .IsRequired();

        // Quantidade como ValueObject
        builder.Property(d => d.Quantidade)
            .HasConversion(
                qtd => qtd.Valor,
                valor => new Quantidade(valor))
            .HasColumnName("Quantidade")
            .IsRequired();

        builder.Property(d => d.DataDistribuicao)
            .HasColumnType("DATETIME")
            .IsRequired();

        builder.HasIndex(d => new { d.ClienteId, d.DataDistribuicao }).HasDatabaseName("IX_Distribuicao_ClienteData");

        // Relacionamento: Cliente 1:N Distribuições
        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureCestaRecomendacao(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<CestaRecomendacao>();

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.DataCriacao)
            .HasColumnType("DATETIME")
            .IsRequired();

        builder.Property(c => c.DataDesativacao)
            .HasColumnType("DATETIME");

        builder.Property(c => c.Ativa)
            .IsRequired()
            .HasDefaultValue(true);

        // Owned Collection para Itens
        builder.OwnsMany(c => c.Itens, nav =>
        {
            nav.ToTable("CestaRecomendacaoItens");
            nav.WithOwner().HasForeignKey("CestaId");
            nav.HasKey("Id");

            // Configure Ticker como ValueObject com conversão
            nav.Property(i => i.Ticker)
                .HasConversion(
                    ticker => ticker.Valor,
                    valor => new Ticker(valor))
                .HasColumnName("Ticker")
                .HasMaxLength(10);

            // Configure Percentual como ValueObject com conversão
            nav.Property(i => i.Percentual)
                .HasConversion(
                    pct => pct.Valor,
                    valor => new Percentual(valor))
                .HasColumnName("Percentual")
                .HasColumnType("DECIMAL(5,2)");
        });
    }

    private void ConfigureOrdemCompra(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<OrdemCompra>();

        builder.HasKey(oc => oc.Id);
        builder.Property(oc => oc.Id).ValueGeneratedOnAdd();

        builder.Property(oc => oc.ContaMasterId)
            .IsRequired();

        // Ticker como ValueObject
        builder.Property(oc => oc.Ticker)
            .HasConversion(
                ticker => ticker.Valor,
                valor => new Ticker(valor))
            .HasColumnName("Ticker")
            .HasMaxLength(10)
            .IsRequired();

        // Quantidade como ValueObject
        builder.Property(oc => oc.Quantidade)
            .HasConversion(
                qtd => qtd.Valor,
                valor => new Quantidade(valor))
            .HasColumnName("Quantidade")
            .IsRequired();

        builder.Property(oc => oc.PrecoUnitario)
            .HasColumnType("DECIMAL(18,4)")
            .IsRequired();

        builder.Property(oc => oc.TipoMercado)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(oc => oc.DataExecucao)
            .HasColumnType("DATETIME")
            .IsRequired();

        builder.HasIndex(oc => new { oc.ContaMasterId, oc.DataExecucao }).HasDatabaseName("IX_OrdemCompra_ContaMasterData");

        // Relacionamento: OrdemCompra 1:N Distribuicoes
        builder.HasMany<Distribuicao>()
            .WithOne()
            .HasForeignKey(d => d.OrdemCompraId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureCotacao(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Cotacao>();

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        // Ticker como ValueObject
        builder.Property(c => c.Ticker)
            .HasConversion(
                ticker => ticker.Valor,
                valor => new Ticker(valor))
            .HasColumnName("Ticker")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(c => c.DataPregao)
            .HasColumnType("DATE")
            .IsRequired();

        builder.Property(c => c.PrecoAbertura)
            .HasColumnType("DECIMAL(18,4)")
            .IsRequired();

        builder.Property(c => c.PrecoFechamento)
            .HasColumnType("DECIMAL(18,4)")
            .IsRequired();

        builder.Property(c => c.PrecoMaximo)
            .HasColumnType("DECIMAL(18,4)")
            .IsRequired();

        builder.Property(c => c.PrecoMinimo)
            .HasColumnType("DECIMAL(18,4)")
            .IsRequired();

        // Índice composto para buscar cotações por ticker e data
        builder.HasIndex("Ticker", "DataPregao").IsUnique().HasDatabaseName("UX_Cotacao_TickerData");
    }

    private void ConfigureEventoIR(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<EventoIR>();

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.ClienteId)
            .IsRequired();

        builder.Property(e => e.Tipo)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.ValorBase)
            .HasColumnType("DECIMAL(18,2)")
            .IsRequired();

        builder.Property(e => e.ValorIR)
            .HasColumnType("DECIMAL(18,2)")
            .IsRequired();

        builder.Property(e => e.PublicadoKafka)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DataEvento)
            .HasColumnType("DATETIME")
            .IsRequired();

        builder.HasIndex(e => new { e.ClienteId, e.DataEvento }).HasDatabaseName("IX_EventoIR_ClienteData");
    }

    private void ConfigureRebalanceamento(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Rebalanceamento>();

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.ClienteId)
            .IsRequired();

        builder.Property(r => r.Tipo)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // TickerVendido como ValueObject (nullable)
        builder.Property(r => r.TickerVendido)
            .HasConversion(
                ticker => ticker != null ? ticker.Valor : null,
                valor => !string.IsNullOrEmpty(valor) ? new Ticker(valor) : null)
            .HasColumnName("TickerVendido")
            .HasMaxLength(10);

        // TickerComprado como ValueObject (nullable)
        builder.Property(r => r.TickerComprado)
            .HasConversion(
                ticker => ticker != null ? ticker.Valor : null,
                valor => !string.IsNullOrEmpty(valor) ? new Ticker(valor) : null)
            .HasColumnName("TickerComprado")
            .HasMaxLength(10);

        builder.Property(r => r.ValorVenda)
            .HasColumnType("DECIMAL(18,2)")
            .IsRequired();

        builder.Property(r => r.DataRebalanceamento)
            .HasColumnType("DATETIME")
            .IsRequired();

        builder.HasIndex(r => new { r.ClienteId, r.DataRebalanceamento }).HasDatabaseName("IX_Rebalanceamento_ClienteData");
    }
}
