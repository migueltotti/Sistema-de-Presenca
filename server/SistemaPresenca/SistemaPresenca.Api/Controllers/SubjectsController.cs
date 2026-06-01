using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Responses.Session;
using SistemaPresenca.Application.UseCases.Queries.Subjects;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/subjects")]
public class SubjectsController(IQueryMediator queryMediator) : ControllerBase
{
    [HttpGet("professor/{id:guid}")]
    public async Task<ActionResult<StartSessionResponse>> GetProfessorSubjectsAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await queryMediator.QueryAsync(new GetProfessorSubjectsQuery(id), cancellationToken);

        return Ok(result);
    }
}
