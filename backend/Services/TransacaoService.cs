using Cgr.Api.Dtos;
using Cgr.Api.Domain.Entities;
using Cgr.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cgr.Api.Services;

/// <summary>
/// Implementa os casos de uso de transacao.
/// </summary>
public class TransacaoService(AppDbContext dbContext) 
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<TransacaoResponse>> ListAsync(CancellationToken cancellationToken)
    {
        // Listagem completa com joins (categoria, pessoa e tipo) para facilitar exibição no front.
        return await dbContext.Transacoes
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Include(t => t.Pessoa)
            .Include(t => t.Tipo)
            .OrderByDescending(t => t.Id)
            .Select(t => new TransacaoResponse
            {
                Id = t.Id,
                Descricao = t.Descricao,
                Valor = t.Valor,
                Tipo = t.Tipo != null ? t.Tipo.Descricao : string.Empty,
                IdCategoria = t.IdCategoria,
                Categoria = t.Categoria!.Descricao,
                IdPessoa = t.IdPessoa,
                Pessoa = t.Pessoa!.Nome
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TransacaoResponse> CreateAsync(TransacaoCreateRequest request, CancellationToken cancellationToken)
    {
        // Regras de entrada mínimas para evitar registros inválidos.
        if (string.IsNullOrWhiteSpace(request.Descricao))
        {
            throw new InvalidOperationException("Descricao da transacao e obrigatoria.");
        }

        // Regra de entrada: o front envia idTipo e validamos na tabela de domínio.
        var tipo = await dbContext.TiposTransacao
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.IdTipo, cancellationToken);

        if (tipo is null)
        {
            throw new InvalidOperationException("Tipo invalido.");
        }

        // Garante integridade referencial: pessoa precisa existir.
        var pessoaExiste = await dbContext.Pessoas.AnyAsync(p => p.Id == request.IdPessoa, cancellationToken);
        if (!pessoaExiste)
        {
            throw new KeyNotFoundException("Pessoa informada nao foi encontrada.");
        }

        // Garante integridade referencial: categoria precisa existir e traz finalidade relacionada.
        var categoria = await dbContext.Categorias
            .AsNoTracking()
            .Include(c => c.Finalidade)
            .FirstOrDefaultAsync(c => c.Id == request.IdCategoria, cancellationToken);

        if (categoria is null)
        {
            throw new KeyNotFoundException("Categoria informada nao foi encontrada.");
        }

        // Regra principal: categoria deve aceitar o tipo da transação ou aceitar "ambas".
        var finalidadeDescricao = categoria.Finalidade?.Descricao ?? string.Empty;
        var categoriaCompativel = finalidadeDescricao == "ambas" || finalidadeDescricao == tipo.Descricao;

        if (!categoriaCompativel)
        {
            throw new InvalidOperationException("A categoria informada nao e compativel com o tipo da transacao.");
        }

        var transacao = new Transacao
        {
            Descricao = request.Descricao.Trim(),
            Valor = request.Valor,
            IdTipo = request.IdTipo,
            IdCategoria = request.IdCategoria,
            IdPessoa = request.IdPessoa
        };

        dbContext.Transacoes.Add(transacao);
        // Salva a transação. Se o cliente cancelar a chamada, o CancellationToken interrompe a operação.
        await dbContext.SaveChangesAsync(cancellationToken);

        // Consulta para retornar dados já "prontos para tela" (nome da pessoa, categoria e tipo textual).
        var transacaoCriada = await dbContext.Transacoes
            .AsNoTracking()
            .Include(t => t.Categoria)
            .Include(t => t.Pessoa)
            .Include(t => t.Tipo)
            .FirstOrDefaultAsync(t => t.Id == transacao.Id, cancellationToken);

        return new TransacaoResponse
        {
            Id = transacaoCriada!.Id,
            Descricao = transacaoCriada.Descricao,
            Valor = transacaoCriada.Valor,
            Tipo = transacaoCriada.Tipo != null ? transacaoCriada.Tipo.Descricao : string.Empty,
            IdCategoria = transacaoCriada.IdCategoria,
            Categoria = transacaoCriada.Categoria!.Descricao,
            IdPessoa = transacaoCriada.IdPessoa,
            Pessoa = transacaoCriada.Pessoa!.Nome
        };
    }
}

