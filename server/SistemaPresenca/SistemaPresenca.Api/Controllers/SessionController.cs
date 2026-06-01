using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Session;
using SistemaPresenca.Application.Responses.Session;
using SistemaPresenca.Application.UseCases.Commands.Sessions;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/session")]
public class SessionController(ICommandMediator commandMediator) : ControllerBase
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
}
