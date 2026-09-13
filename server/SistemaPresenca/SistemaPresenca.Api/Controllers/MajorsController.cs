using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Application.Responses.Majors;
using SistemaPresenca.Application.UseCases.Commands.Majors;
using SistemaPresenca.Application.UseCases.Queries.Majors;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/majors")]
public sealed class MajorsController(ICommandMediator commandMediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetMajorsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMajorsAsync([FromQuery] GetMajorsRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new GetMajorsCommand(request), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateMajorAsync([FromBody] CreateMajorRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new CreateMajorsCommand(request), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }
}
