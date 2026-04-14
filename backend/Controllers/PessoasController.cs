using Cgr.Api.Dtos;
using Cgr.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cgr.Api.Controllers;

/// <summary>
/// Endpoints do cadastro de pessoas.
/// </summary>
[ApiController]
[Route("api/pessoas")]
public class PessoasController(PessoaService pessoaService) : ControllerBase
{
    /// <summary>
    /// Lista todas as pessoas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PessoaResponse>>> GetAll(CancellationToken cancellationToken)
    {
        // cancellationToken: se o cliente fechar a tela, a consulta pode ser interrompida.
        var result = await pessoaService.ListAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova pessoa.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PessoaResponse>> Create([FromBody] PessoaCreateRequest request, CancellationToken cancellationToken)
    {
        // Recebe os dados da tela, manda para o serviço e devolve o registro criado.
        var created = await pessoaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza uma pessoa existente.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PessoaResponse>> Update(int id, [FromBody] PessoaUpdateRequest request, CancellationToken cancellationToken)
    {
        // Atualiza pessoa pelo id informado na URL.
        var updated = await pessoaService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    /// <summary>
    /// Remove uma pessoa por identificador.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        // Exclui pessoa pelo id e devolve 204 (sem conteúdo).
        await pessoaService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}


