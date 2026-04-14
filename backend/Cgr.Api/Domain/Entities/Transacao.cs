namespace Cgr.Api.Domain.Entities;

/// <summary>
/// Registro financeiro de despesa ou receita de uma pessoa.
/// </summary>
public class Transacao
{
    /// <summary>
    /// Identificador unico da transacao.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descricao livre da movimentacao.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor monetario sempre positivo.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Identificador do tipo da transacao.
    /// </summary>
    public int IdTipo { get; set; }

    /// <summary>
    /// Relacao de navegacao com o tipo da transacao.
    /// </summary>
    public TipoTransacao? Tipo { get; set; }

    /// <summary>
    /// Identificador da categoria da transacao.
    /// </summary>
    public int IdCategoria { get; set; }

    /// <summary>
    /// Relacao de navegacao com a categoria.
    /// </summary>
    public Categoria? Categoria { get; set; }

    /// <summary>
    /// Identificador da pessoa vinculada a transacao.
    /// </summary>
    public int IdPessoa { get; set; }

    /// <summary>
    /// Relacao de navegacao com a pessoa.
    /// </summary>
    public Pessoa? Pessoa { get; set; }
}
