using CompraProgramada.Domain.Exceptions;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Entidade Cliente — Pessoa que adere ao programa de compra programada
/// RN-001 a RN-013
/// </summary>
public class Cliente
{
    public long Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public CPF CPF { get; private set; } = null!;
    public string Email { get; private set; } = string.Empty;
    public decimal ValorMensal { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataAdesao { get; private set; }
    public DateTime? DataSaida { get; private set; }

    // EF Core requires a parameterless constructor
    protected Cliente() { }

    /// <summary>
    /// Criar novo cliente (RN-001 a RN-006)
    /// </summary>
    public Cliente(string nome, string cpf, string email, decimal valorMensal)
    {
        ValidarNome(nome);
        ValidarEmail(email);
        ValidarValorMensal(valorMensal);

        Nome = nome;
        CPF = new CPF(cpf);
        Email = email;
        ValorMensal = valorMensal;
        Ativo = true;
        DataAdesao = DateTime.UtcNow;
    }

    /// <summary>
    /// Sair do produto (RN-007, RN-009)
    /// A posição em custódia é mantida (RN-008)
    /// </summary>
    public void Sair()
    {
        if (!Ativo)
            throw new InvalidOperationException("Cliente já estava inativo");

        Ativo = false;
        DataSaida = DateTime.UtcNow;
    }

    /// <summary>
    /// Alterar valor mensal de aporte (RN-011, RN-012, RN-013)
    /// O novo valor é usado a partir da próxima data de compra
    /// </summary>
    public void AlterarValorMensal(decimal novoValor)
    {
        ValidarValorMensal(novoValor);
        ValorMensal = novoValor;
    }

    /// <summary>
    /// Calcula 1/3 do valor mensal para cada data de compra (5, 15, 25)
    /// </summary>
    public decimal CalcularValorParcela()
    {
        return Math.Truncate(ValorMensal / 3 * 100) / 100; // Truncar em 2 casas decimais
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio", nameof(nome));

        if (nome.Length > 200)
            throw new ArgumentException("Nome não pode exceder 200 caracteres", nameof(nome));
    }

    private static void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio", nameof(email));

        if (email.Length > 200)
            throw new ArgumentException("Email não pode exceder 200 caracteres", nameof(email));

        // Validação básica de email
        if (!email.Contains("@") || !email.Contains("."))
            throw new ArgumentException("Email inválido", nameof(email));
    }

    private static void ValidarValorMensal(decimal valor)
    {
        const decimal valorMinimo = 100m;

        if (valor < valorMinimo)
            throw new ValorMensalInvalidoException(valor, valorMinimo);
    }

    public override string ToString() => $"Cliente: {Nome} ({CPF})";
}
