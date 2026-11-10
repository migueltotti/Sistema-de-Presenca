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

    // TODO: Modificar esse endpoint para ele ser o "INICIAR REGISTRO DE PRESENÇA MANUAL"
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

    // Esse endpoint só será chamado pela interface web, para finalizar a aula ao fechar a porta o endpoint é o /api/v1/rooms//microcontroller/{microcontrollerId}/close/tag-id/{tagId}
    [HttpPost("{id:guid}/end")]
    [Authorize("AdminOrProfessor")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> EndSessionAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new EndSessionCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    // TODO: modificar esse endpoint para passar o id do usuário/aluno na rota e criar outro endpoint passando o tagId do aluno no body ou na rota.
    // Vamos separar as rotinas de registrar presença pelo esp e pela interface web.
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
