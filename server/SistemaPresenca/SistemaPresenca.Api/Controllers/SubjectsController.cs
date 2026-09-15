using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Application.Responses.Majors;
using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Application.UseCases.Commands.Majors;
using SistemaPresenca.Application.UseCases.Commands.Subjects;
using SistemaPresenca.Application.UseCases.Queries.Majors;
using SistemaPresenca.Application.UseCases.Queries.Subjects;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/subjects")]
public class SubjectsController(ICommandMediator commandMediator, IQueryMediator queryMediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetSubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSubjectsAsync([FromQuery] GetSubjectsRequest request, CancellationToken cancellationToken)
    {
        var result = await queryMediator.QueryAsync(new GetSubjectsQuery(request), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSubjectAsync([FromBody] CreateSubjectRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new CreateSubjectCommand(request), cancellationToken);

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
    public async Task<IActionResult> DeleteSubjectAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new DeleteSubjectCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }
}
