namespace Cgr.Api.Domain.Entities;

/// <summary>
/// Tabela de dominio com os tipos permitidos de transacao.
/// </summary>
public class TipoTransacao
{
    /// <summary>
    /// Identificador unico do tipo.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descricao textual do tipo.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Transacoes que usam esse tipo.
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
