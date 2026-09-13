using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Application.Responses.Majors;
using SistemaPresenca.Application.UseCases.Commands.Majors;
using SistemaPresenca.Application.UseCases.Queries.Majors;
using SistemaPresenca.Domain.Errors;
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
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateMajorAsync([FromBody] CreateMajorRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new CreateMajorsCommand(request), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Consumes("application/json-patch+json")]
    public async Task<IActionResult> UpdateMajorAsync([FromRoute] Guid id, [FromBody] JsonPatchDocument<GetMajorsResponse> request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new UpdateMajorCommand(id, new(request)), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMajorAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new DeleteMajorCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }
}
