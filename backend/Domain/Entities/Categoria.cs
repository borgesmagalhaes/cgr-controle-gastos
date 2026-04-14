namespace Cgr.Api.Domain.Entities;

/// <summary>
/// Define uma categoria de classificacao de transacoes.
/// </summary>
public class Categoria
{
    /// <summary>
    /// Identificador unico da categoria.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Texto descritivo da categoria.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da finalidade aceita pela categoria.
    /// </summary>
    public int IdFinalidade { get; set; }

    /// <summary>
    /// Relacao de navegacao com a finalidade.
    /// </summary>
    public Finalidade? Finalidade { get; set; }

    /// <summary>
    /// Colecao de transacoes associadas a categoria.
    /// </summary>
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
