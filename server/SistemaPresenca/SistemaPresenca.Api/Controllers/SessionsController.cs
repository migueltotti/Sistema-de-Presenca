using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Session;
using SistemaPresenca.Application.Responses.Session;
using SistemaPresenca.Application.UseCases.Commands.Sessions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/sessions")]
public class SessionsController(ICommandMediator commandMediator) : ControllerBase
{
    [HttpPost]
    [Authorize("AdminOrProfessor")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    [Authorize("AdminOrProfessor")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
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
    [Authorize("AdminOrProfessor")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> EndSessionAsync([FromRoute] Guid id, [FromBody] EndSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new EndSessionCommand(id, request.ProfessorTagId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("{id:guid}/attendance")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RegisterAttendanceAsync([FromRoute] Guid id, [FromBody] RegisterAttendanceRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new RegisterAttendanceCommand(id, request.StudentTagId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}
