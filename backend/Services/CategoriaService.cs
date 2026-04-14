using Cgr.Api.Dtos;
using Cgr.Api.Domain.Entities;
using Cgr.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cgr.Api.Services;

/// <summary>
/// Implementa os casos de uso de categoria.
/// </summary>
public class CategoriaService(AppDbContext dbContext) 
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoriaResponse>> ListAsync(CancellationToken cancellationToken)
    {
        // Retorna categorias já com a descrição da finalidade (vinda da tabela relacionada).
        return await dbContext.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Descricao)
            .Select(c => new CategoriaResponse
            {
                Id = c.Id,
                Descricao = c.Descricao,
                IdFinalidade = c.IdFinalidade,
                Finalidade = c.Finalidade != null ? c.Finalidade.Descricao : string.Empty
            })
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CategoriaResponse> CreateAsync(CategoriaCreateRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Descricao))
        {
            throw new InvalidOperationException("Descricao da categoria e obrigatoria.");
        }

        // Valida se a finalidade informada existe de verdade no banco (FK real).
        var finalidadeExiste = await dbContext.Finalidades
            .AsNoTracking()
            .AnyAsync(f => f.Id == request.IdFinalidade, cancellationToken);

        if (!finalidadeExiste)
        {
            throw new InvalidOperationException("Finalidade invalida.");
        }

        var categoria = new Categoria
        {
            Descricao = request.Descricao.Trim(),
            IdFinalidade = request.IdFinalidade
        };

        dbContext.Categorias.Add(categoria);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Busca a descrição da finalidade para devolver uma resposta amigável ao front.
        var finalidadeDescricao = await dbContext.Finalidades
            .AsNoTracking()
            .Where(f => f.Id == categoria.IdFinalidade)
            .Select(f => f.Descricao)
            .FirstOrDefaultAsync(cancellationToken);

        return new CategoriaResponse
        {
            Id = categoria.Id,
            Descricao = categoria.Descricao,
            IdFinalidade = categoria.IdFinalidade,
            Finalidade = finalidadeDescricao ?? string.Empty
        };
    }
}


