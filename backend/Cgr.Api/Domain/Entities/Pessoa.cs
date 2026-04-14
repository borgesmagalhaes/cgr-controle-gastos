namespace Cgr.Api.Domain.Entities;

/// <summary>
/// Representa a pessoa dona das transacoes no sistema.
/// </summary>
public class Pessoa
{
    /// <summary>
    /// Identificador unico da pessoa.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome completo da pessoa.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Idade atual da pessoa.
    /// </summary>
    public int Idade { get; set; }

    /// <summary>
    /// Colecao de transacoes vinculadas a pessoa.
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
