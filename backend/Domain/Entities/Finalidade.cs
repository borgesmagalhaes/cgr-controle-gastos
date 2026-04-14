namespace Cgr.Api.Domain.Entities;

/// <summary>
/// Tabela de dominio com as finalidades possiveis da categoria.
/// </summary>
public class Finalidade
{
    /// <summary>
    /// Identificador unico da finalidade.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descricao textual da finalidade.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Categorias que utilizam essa finalidade.
    /// </summary>
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
}
