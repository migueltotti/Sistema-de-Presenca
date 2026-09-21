using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Application.Requests.Users;
using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Application.Responses.Users;
using SistemaPresenca.Application.UseCases.Commands.Users;
using SistemaPresenca.Application.UseCases.Queries.Subjects;
using SistemaPresenca.Application.UseCases.Queries.Users;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController(ICommandMediator commandMediator, IQueryMediator queryMediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsersAsync([FromQuery] GetUsersRequest request, CancellationToken cancellationToken)
    {
        var result = await queryMediator.QueryAsync(new GetUsersQuery(request), cancellationToken);

        return Ok(result);
    }

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
