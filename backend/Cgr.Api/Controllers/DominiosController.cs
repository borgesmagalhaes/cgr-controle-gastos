using Cgr.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cgr.Api.Controllers;

/// <summary>
/// Endpoints de apoio para valores de dominio.
/// </summary>
[ApiController]
[Route("api/dominios")]
public class DominiosController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Lista finalidades disponiveis.
    /// </summary>
    [HttpGet("finalidades")]
    public async Task<ActionResult<IReadOnlyList<DominioItemResponse>>> GetFinalidades(CancellationToken cancellationToken)
    {
        var finalidades = await dbContext.Finalidades
            .AsNoTracking()
            .OrderBy(f => f.Id)
            .Select(f => new DominioItemResponse
            {
                Id = f.Id,
                Descricao = f.Descricao
            })
            .ToListAsync(cancellationToken);

        return Ok(finalidades);
    }

    /// <summary>
    /// Lista tipos de transacao disponiveis.
    /// </summary>
    [HttpGet("tipos-transacao")]
    public async Task<ActionResult<IReadOnlyList<DominioItemResponse>>> GetTiposTransacao(CancellationToken cancellationToken)
    {
        var tipos = await dbContext.TiposTransacao
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .Select(t => new DominioItemResponse
            {
                Id = t.Id,
                Descricao = t.Descricao
            })
            .ToListAsync(cancellationToken);

        return Ok(tipos);
    }
}

/// <summary>
/// Item simples de dominio para exibir id + descricao.
/// </summary>
public class DominioItemResponse
{
    /// <summary>
    /// Identificador do item de dominio.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Descricao do item de dominio.
    /// </summary>
    public string Descricao { get; set; } = string.Empty;
}
