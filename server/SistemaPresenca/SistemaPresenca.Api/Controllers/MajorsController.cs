using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Application.UseCases.Commands.Majors;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/majors")]
public sealed class MajorsController(ICommandMediator commandMediator) : ControllerBase
{
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
