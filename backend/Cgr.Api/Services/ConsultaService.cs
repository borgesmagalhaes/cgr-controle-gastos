using Cgr.Api.Dtos;
using Cgr.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cgr.Api.Services;

/// <summary>
/// Implementa consultas consolidadas para relatarios de totais.
/// </summary>
public class ConsultaService(AppDbContext dbContext) 
{
    /// <inheritdoc />
    public async Task<ConsultaTotaisPorPessoaResult> GetTotaisPorPessoaAsync(CancellationToken cancellationToken)
    {
        // Busca os ids de despesa/receita no banco para não depender de números "fixos no código".
        var (tipoDespesaId, tipoReceitaId) = await GetTiposAsync(cancellationToken);

        // Para cada pessoa, soma receitas e despesas em consultas agregadas.
        var pessoas = await dbContext.Pessoas
            .AsNoTracking()
            .Select(p => new TotalPorPessoaResponse
            {
                PessoaId = p.Id,
                Nome = p.Nome,
                TotalReceitas = dbContext.Transacoes
                    .Where(t => t.IdPessoa == p.Id && t.IdTipo == tipoReceitaId)
                    .Sum(t => (decimal?)t.Valor) ?? 0m,
                TotalDespesas = dbContext.Transacoes
                    .Where(t => t.IdPessoa == p.Id && t.IdTipo == tipoDespesaId)
                    .Sum(t => (decimal?)t.Valor) ?? 0m
            })
            .OrderBy(p => p.Nome)
            .ToListAsync(cancellationToken);

        foreach (var pessoa in pessoas)
        {
            // Saldo individual = total de receitas - total de despesas.
            pessoa.Saldo = pessoa.TotalReceitas - pessoa.TotalDespesas;
        }

        return new ConsultaTotaisPorPessoaResult
        {
            Pessoas = pessoas,
            TotalGeral = BuildTotaisGerais(pessoas.Select(p => p.TotalReceitas), pessoas.Select(p => p.TotalDespesas))
        };
    }

    /// <inheritdoc />
    public async Task<ConsultaTotaisPorCategoriaResult> GetTotaisPorCategoriaAsync(CancellationToken cancellationToken)
    {
        var (tipoDespesaId, tipoReceitaId) = await GetTiposAsync(cancellationToken);

        // Para cada categoria, soma receitas e despesas.
        var categorias = await dbContext.Categorias
            .AsNoTracking()
            .Select(c => new TotalPorCategoriaResponse
            {
                CategoriaId = c.Id,
                Descricao = c.Descricao,
                TotalReceitas = dbContext.Transacoes
                    .Where(t => t.IdCategoria == c.Id && t.IdTipo == tipoReceitaId)
                    .Sum(t => (decimal?)t.Valor) ?? 0m,
                TotalDespesas = dbContext.Transacoes
                    .Where(t => t.IdCategoria == c.Id && t.IdTipo == tipoDespesaId)
                    .Sum(t => (decimal?)t.Valor) ?? 0m
            })
            .OrderBy(c => c.Descricao)
            .ToListAsync(cancellationToken);

        foreach (var categoria in categorias)
        {
            // Saldo por categoria = receitas - despesas.
            categoria.Saldo = categoria.TotalReceitas - categoria.TotalDespesas;
        }

        return new ConsultaTotaisPorCategoriaResult
        {
            Categorias = categorias,
            TotalGeral = BuildTotaisGerais(categorias.Select(c => c.TotalReceitas), categorias.Select(c => c.TotalDespesas))
        };
    }

    /// <summary>
    /// Monta o bloco de totais gerais reutilizado pelas consultas.
    /// </summary>
    private static TotaisGeraisResponse BuildTotaisGerais(IEnumerable<decimal> receitas, IEnumerable<decimal> despesas)
    {
        var totalReceitas = receitas.Sum();
        var totalDespesas = despesas.Sum();

        return new TotaisGeraisResponse
        {
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas,
            SaldoLiquido = totalReceitas - totalDespesas
        };
    }

    /// <summary>
    /// Busca os ids reais dos tipos de transacao na tabela de dominio.
    /// </summary>
    private async Task<(int tipoDespesaId, int tipoReceitaId)> GetTiposAsync(CancellationToken cancellationToken)
    {
        // Lê a tabela de domínio tipo_transacao para localizar ids reais.
        var tipos = await dbContext.TiposTransacao
            .AsNoTracking()
            .Select(t => new { t.Id, t.Descricao })
            .ToListAsync(cancellationToken);

        var tipoDespesaId = tipos
            .Where(t => t.Descricao == "despesa")
            .Select(t => t.Id)
            .FirstOrDefault();

        var tipoReceitaId = tipos
            .Where(t => t.Descricao == "receita")
            .Select(t => t.Id)
            .FirstOrDefault();

        if (tipoDespesaId <= 0 || tipoReceitaId <= 0)
        {
            // Sem esses tipos não existe base consistente para calcular os totais.
            throw new InvalidOperationException("Tipos de transacao (despesa/receita) nao encontrados no banco.");
        }

        return (tipoDespesaId, tipoReceitaId);
    }
}


