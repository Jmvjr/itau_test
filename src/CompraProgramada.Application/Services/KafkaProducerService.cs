using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Application.Services;

/// <summary>
/// Implementação de um mock de KafkaProducerService para desenvolvimento
/// Em produção, seria substituído por implementação real com Confluent.Kafka
/// </summary>
public class KafkaProducerService : IKafkaProducerService
{
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly string _bootstrapServers;

    public KafkaProducerService(
        ILogger<KafkaProducerService> logger,
        string bootstrapServers = "localhost:9092")
    {
        _logger = logger;
        _bootstrapServers = bootstrapServers;
    }

    public async Task PublicarAsync(string topico, string chave, object mensagem)
    {
        try
        {
            var json = JsonSerializer.Serialize(mensagem, new JsonSerializerOptions 
            { 
                WriteIndented = false 
            });

            _logger.LogInformation(
                "Publicando mensagem no Kafka - Tópico: {Topico}, Chave: {Chave}, Mensagem: {Mensagem}",
                topico,
                chave,
                json
            );

            // Simular envio assíncrono
            await Task.Delay(10);

            _logger.LogInformation(
                "Mensagem publicada com sucesso no tópico '{Topico}'",
                topico
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no Kafka - Tópico: {Topico}", topico);
            throw new InvalidOperationException($"Falha ao publicar no Kafka tópico '{topico}'", ex);
        }
    }

    public async Task<bool> EstaConectadoAsync()
    {
        try
        {
            // Implementar verificação real com Kafka quando tiver Confluent SDK
            _logger.LogInformation("Verificando conexão com Kafka em {Server}", _bootstrapServers);
            await Task.Delay(10);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Não conseguiu conectar ao Kafka");
            return false;
        }
    }
}
