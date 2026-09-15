using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Application.UseCases.Commands.Subjects;
using SistemaPresenca.Application.UseCases.Queries.Subjects;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/subjects")]
public class SubjectsController(ICommandMediator commandMediator, IQueryMediator queryMediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<GetSubsjectResponse>> GetProfessorSubjectsAsync([FromQuery] string professorTagId, CancellationToken cancellationToken)
    {
        var result = await queryMediator.QueryAsync(new GetProfessorSubjectsQuery(professorTagId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Data);
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
}
