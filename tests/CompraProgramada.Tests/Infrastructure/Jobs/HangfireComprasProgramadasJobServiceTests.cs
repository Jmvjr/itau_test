using Xunit;
using Moq;
using CompraProgramada.Application.Services;
using CompraProgramada.Application.Jobs;
using CompraProgramada.Infrastructure.Jobs;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Tests.Infrastructure.Jobs;

public class HangfireComprasProgramadasJobServiceTests
{
    private readonly Mock<IComprasProgramadasService> _comprasServiceMock;
    private readonly Mock<ILogger<HangfireComprasProgramadasJobService>> _loggerMock;
    private readonly HangfireComprasProgramadasJobService _jobService;

    public HangfireComprasProgramadasJobServiceTests()
    {
        _comprasServiceMock = new Mock<IComprasProgramadasService>();
        _loggerMock = new Mock<ILogger<HangfireComprasProgramadasJobService>>();

        _jobService = new HangfireComprasProgramadasJobService(
            _comprasServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ProcessarComprasAsync_ExecutaSemErro()
    {
        // Arrange
        _comprasServiceMock
            .Setup(s => s.ProcessarComprasAgrupadasAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(5);

        // Act - Deve executar sem lançar exceção
        await _jobService.ProcessarComprasAsync();

        // Assert - Teste passou se chegou aqui
        Assert.True(true);
    }

    [Fact]
    public async Task ObterJobsRegistradosAsync_RetornaJobsRegistrados()
    {
        // Act
        var jobs = await _jobService.ObterJobsRegistradosAsync();

        // Assert
        Assert.NotNull(jobs);
        Assert.Equal(3, jobs.Count()); // 3 jobs (5, 15, 25)
        Assert.All(jobs, j => Assert.NotNull(j.Id));
        Assert.All(jobs, j => Assert.NotNull(j.Descricao));
        Assert.All(jobs, j => Assert.Equal("Ativo", j.Status));
    }

    [Fact]
    public async Task ObterJobsRegistradosAsync_TemDias5_15_25()
    {
        // Act
        var jobs = await _jobService.ObterJobsRegistradosAsync();

        // Assert
        var ids = jobs.Select(j => j.Id).ToList();
        Assert.Contains("compras-programadas-dia-5", ids);
        Assert.Contains("compras-programadas-dia-15", ids);
        Assert.Contains("compras-programadas-dia-25", ids);
    }

    [Fact]
    public async Task ObterJobsRegistradosAsync_TodosComDescricaoAlusiva()
    {
        // Act
        var jobs = await _jobService.ObterJobsRegistradosAsync();

        // Assert
        Assert.All(jobs, jobs => 
            Assert.Contains("Processamento automático de compras", jobs.Descricao));
    }
}
