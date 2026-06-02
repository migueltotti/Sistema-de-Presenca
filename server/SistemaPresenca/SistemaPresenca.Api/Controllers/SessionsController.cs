using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Session;
using SistemaPresenca.Application.Responses.Session;
using SistemaPresenca.Application.UseCases.Commands.Sessions;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/sessions")]
public class SessionsController(ICommandMediator commandMediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<StartSessionResponse>> StartSessionAsync([FromBody] StartSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new StartSessionCommand(request), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/continue")]
    public async Task<ActionResult> ContinueSessionAsync([FromRoute] Guid id, [FromBody] ContinueSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new ContinueSessionCommand(id, request.ProfessorTagId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("{id:guid}/end")]
    public async Task<ActionResult> EndSessionAsync([FromRoute] Guid id, [FromBody] EndSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new EndSessionCommand(id, request.ProfessorTagId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}
