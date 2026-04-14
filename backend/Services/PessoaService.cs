using Cgr.Api.Dtos;
using Cgr.Api.Domain.Entities;
using Cgr.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cgr.Api.Services;

/// <summary>
/// Implementa os casos de uso de pessoa.
/// </summary>
public class PessoaService(AppDbContext dbContext) 
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<PessoaResponse>> ListAsync(CancellationToken cancellationToken)
    {
        // CancellationToken: se a requisição for cancelada (ex.: usuário fecha a tela),
        // o EF Core interrompe a consulta e evita processamento desnecessário no servidor.
        return await dbContext.Pessoas
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .Select(p => new PessoaResponse
            {
                Id = p.Id,
                Nome = p.Nome,
                Idade = p.Idade
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PessoaResponse> CreateAsync(PessoaCreateRequest request, CancellationToken cancellationToken)
    {
        // Regra básica de negócio: não permitimos pessoa sem nome.
        ValidatePessoa(request.Nome, request.Idade);

        var pessoa = new Pessoa
        {
            Nome = request.Nome.Trim(),
            Idade = request.Idade
        };

        dbContext.Pessoas.Add(pessoa);
        // SaveChangesAsync também recebe CancellationToken para interromper o salvamento
        // caso a chamada HTTP seja cancelada.
        await dbContext.SaveChangesAsync(cancellationToken);

        return new PessoaResponse
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Idade = pessoa.Idade
        };
    }

    /// <inheritdoc />
    public async Task<PessoaResponse> UpdateAsync(int id, PessoaUpdateRequest request, CancellationToken cancellationToken)
    {
        ValidatePessoa(request.Nome, request.Idade);

        // Busca a pessoa pelo id para atualização.
        var pessoa = await dbContext.Pessoas.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (pessoa is null)
        {
            throw new KeyNotFoundException("Pessoa nao encontrada.");
        }

        pessoa.Nome = request.Nome.Trim();
        pessoa.Idade = request.Idade;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new PessoaResponse
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Idade = pessoa.Idade
        };
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        // Remove a pessoa. As transações dela são removidas em cascata pelo relacionamento do banco.
        var pessoa = await dbContext.Pessoas.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (pessoa is null)
        {
            throw new KeyNotFoundException("Pessoa nao encontrada.");
        }

        dbContext.Pessoas.Remove(pessoa);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Aplica validacoes de dominio de pessoa em um unico ponto.
    /// </summary>
    private static void ValidatePessoa(string nome, int idade)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new InvalidOperationException("Nome da pessoa e obrigatorio.");
        }
    }
}


