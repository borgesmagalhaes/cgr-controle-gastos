namespace Cgr.Api.Dtos;

/// <summary>
/// Totais consolidados por pessoa.
/// </summary>
public class TotalPorPessoaResponse
{
    /// <summary>
    /// Identificador da pessoa.
    /// </summary>
    public int PessoaId { get; set; }

    /// <summary>
    /// Nome da pessoa.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Soma de receitas da pessoa.
    /// </summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>
    /// Soma de despesas da pessoa.
    /// </summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>
    /// Saldo liquido da pessoa.
    /// </summary>
    public decimal Saldo { get; set; }
}

/// <summary>
/// Totais consolidados por categoria.
/// </summary>
public class TotalPorCategoriaResponse
{
    /// <summary>
    /// Identificador da categoria.
    /// </summary>
    public int CategoriaId { get; set; }

    /// <summary>
    /// Descricao da categoria.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Soma de receitas da categoria.
    /// </summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>
    /// Soma de despesas da categoria.
    /// </summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>
    /// Saldo liquido da categoria.
    /// </summary>
    public decimal Saldo { get; set; }
}

/// <summary>
/// Bloco de total geral retornado nas consultas.
/// </summary>
public class TotaisGeraisResponse
{
    /// <summary>
    /// Total geral de receitas.
    /// </summary>
    public decimal TotalReceitas { get; set; }

    /// <summary>
    /// Total geral de despesas.
    /// </summary>
    public decimal TotalDespesas { get; set; }

    /// <summary>
    /// Saldo liquido geral.
    /// </summary>
    public decimal SaldoLiquido { get; set; }
}

/// <summary>
/// Resposta da consulta de totais por pessoa com bloco final geral.
/// </summary>
public class ConsultaTotaisPorPessoaResult
{
    /// <summary>
    /// Lista de totais por pessoa.
    /// </summary>
    public IReadOnlyList<TotalPorPessoaResponse> Pessoas { get; set; } = [];

    /// <summary>
    /// Bloco de total geral da consulta.
    /// </summary>
    public TotaisGeraisResponse TotalGeral { get; set; } = new();
}

/// <summary>
/// Resposta da consulta de totais por categoria com bloco final geral.
/// </summary>
public class ConsultaTotaisPorCategoriaResult
{
    /// <summary>
    /// Lista de totais por categoria.
    /// </summary>
    public IReadOnlyList<TotalPorCategoriaResponse> Categorias { get; set; } = [];

    /// <summary>
    /// Bloco de total geral da consulta.
    /// </summary>
    public TotaisGeraisResponse TotalGeral { get; set; } = new();
}

