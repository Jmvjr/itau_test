namespace CompraProgramada.Application.Services;

/// <summary>
/// Serviço para publicar mensagens em tópicos Kafka
/// Usado para publicar eventos de IR e outras integrações
/// </summary>
public interface IKafkaProducerService
{
    /// <summary>
    /// Publica uma mensagem em um tópico Kafka
    /// </summary>
    /// <param name="topico">Nome do tópico Kafka</param>
    /// <param name="chave">Chave da mensagem (usado para particionamento)</param>
    /// <param name="mensagem">Objeto com os dados da mensagem</param>
    Task PublicarAsync(string topico, string chave, object mensagem);

    /// <summary>
    /// Verifica se o serviço está conectado ao Kafka
    /// </summary>
    Task<bool> EstaConectadoAsync();
}
