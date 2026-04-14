using Cgr.Api.Dtos;
using Cgr.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cgr.Api.Controllers;

/// <summary>
/// Endpoints do cadastro de transacoes.
/// </summary>
[ApiController]
[Route("api/transacoes")]
public class TransacoesController(TransacaoService transacaoService) : ControllerBase
{
    /// <summary>
    /// Lista todas as transacoes.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TransacaoResponse>>> GetAll(CancellationToken cancellationToken)
    {
        // cancellationToken: evita continuar a leitura se o usuário cancelar a chamada.
        var result = await transacaoService.ListAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Cria uma nova transacao.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransacaoResponse>> Create([FromBody] TransacaoCreateRequest request, CancellationToken cancellationToken)
    {
        // Cria transação e devolve o resultado já pronto para exibição na tela.
        var created = await transacaoService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }
}


