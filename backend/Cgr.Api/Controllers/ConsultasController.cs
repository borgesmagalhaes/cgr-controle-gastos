using Cgr.Api.Dtos;
using Cgr.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cgr.Api.Controllers;

/// <summary>
/// Endpoints de consultas consolidadas de totais.
/// </summary>
[ApiController]
[Route("api/consultas")]
public class ConsultasController(ConsultaService consultaService) : ControllerBase
{
    /// <summary>
    /// Consulta totais por pessoa com total geral no final.
    /// </summary>
    [HttpGet("totais-por-pessoa")]
    public async Task<ActionResult<ConsultaTotaisPorPessoaResult>> GetTotaisPorPessoa(CancellationToken cancellationToken)
    {
        // Retorna lista por pessoa + linha de total geral no final.
        var result = await consultaService.GetTotaisPorPessoaAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Consulta totais por categoria com total geral no final.
    /// </summary>
    [HttpGet("totais-por-categoria")]
    public async Task<ActionResult<ConsultaTotaisPorCategoriaResult>> GetTotaisPorCategoria(CancellationToken cancellationToken)
    {
        // Retorna lista por categoria + linha de total geral no final.
        var result = await consultaService.GetTotaisPorCategoriaAsync(cancellationToken);
        return Ok(result);
    }
}


