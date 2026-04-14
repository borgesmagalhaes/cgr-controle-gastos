using Cgr.Api.Dtos;
using Cgr.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cgr.Api.Controllers;

/// <summary>
/// Endpoints do cadastro de categorias.
/// </summary>
[ApiController]
[Route("api/categorias")]
public class CategoriasController(CategoriaService categoriaService) : ControllerBase
{
    /// <summary>
    /// Lista todas as categorias.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoriaResponse>>> GetAll(CancellationToken cancellationToken)
    {
        // cancellationToken: permite parar a consulta se a requisição for cancelada.
        var result = await categoriaService.ListAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova categoria.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoriaResponse>> Create([FromBody] CategoriaCreateRequest request, CancellationToken cancellationToken)
    {
        // Cria categoria com os dados enviados no corpo da requisição.
        var created = await categoriaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }
}


