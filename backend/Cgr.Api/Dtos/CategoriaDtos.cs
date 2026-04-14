namespace Cgr.Api.Dtos;

/// <summary>
/// Payload para criar categoria.
/// </summary>
public class CategoriaCreateRequest
{
    /// <summary>
    /// Descricao com limite de 400 caracteres.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da finalidade (FK para tabela finalidade).
    /// </summary>
    public int IdFinalidade { get; set; }
}

/// <summary>
/// Resposta de leitura de categoria.
/// </summary>
public class CategoriaResponse
{
    /// <summary>
    /// Identificador da categoria.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descricao da categoria.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Finalidade em texto da categoria.
    /// </summary>
    public string Finalidade { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da finalidade.
    /// </summary>
    public int IdFinalidade { get; set; }
}

