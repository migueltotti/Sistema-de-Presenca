using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Users;
using SistemaPresenca.Application.UseCases.Commands.Users;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(ICommandMediator commandMediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSubjectAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new CreateUserCommand(request), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }
}
