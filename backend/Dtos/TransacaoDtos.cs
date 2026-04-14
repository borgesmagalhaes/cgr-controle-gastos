namespace Cgr.Api.Dtos;

/// <summary>
/// Payload para criar transacao.
/// </summary>
public class TransacaoCreateRequest
{
    /// <summary>
    /// Descricao com limite de 400 caracteres.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor positivo da transacao.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Identificador do tipo (FK para tabela tipo_transacao).
    /// </summary>
    public int IdTipo { get; set; }

    /// <summary>
    /// Identificador da categoria.
    /// </summary>
    public int IdCategoria { get; set; }

    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public int IdPessoa { get; set; }
}

/// <summary>
/// Resposta de leitura de transacao.
/// </summary>
public class TransacaoResponse
{
    /// <summary>
    /// Identificador da transacao.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descricao da transacao.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Valor da transacao.
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Tipo em texto da transacao.
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da categoria.
    /// </summary>
    public int IdCategoria { get; set; }

    /// <summary>
    /// Descricao da categoria.
    /// </summary>
    public string Categoria { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public int IdPessoa { get; set; }

    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    public string Pessoa { get; set; } = string.Empty;
}

