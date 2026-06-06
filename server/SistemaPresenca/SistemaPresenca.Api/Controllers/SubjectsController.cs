using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Application.UseCases.Queries.Subjects;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/subjects")]
public class SubjectsController(IQueryMediator queryMediator) : ControllerBase
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
}
