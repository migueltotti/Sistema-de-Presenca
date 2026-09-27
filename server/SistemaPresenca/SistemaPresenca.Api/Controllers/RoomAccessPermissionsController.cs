using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.RoomAccessPermissions;
using SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/rooms/{roomId:guid}/access-permissions")]
public class RoomAccessPermissionsController(ICommandMediator commandMediator) : ControllerBase
{
    // TODO: restringir este endpoint a usuários com Role == UserRole.Admin quando a autenticação for implementada.
    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GrantAccessAsync(
        [FromRoute] Guid roomId,
        [FromBody] RoomAccessRequest request,
        CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(
            new GrantRoomAccessCommand(request.UserId, roomId, null),
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }

    // TODO: restringir este endpoint a usuários com Role == UserRole.Admin quando a autenticação for implementada.
    [HttpDelete("{permissionId:guid}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RevokeAccessAsync([FromRoute] Guid permissionId, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new RevokeRoomAccessCommand(permissionId), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }

    // TODO: restringir este endpoint a usuários com Role == UserRole.Admin quando a autenticação for implementada.
    [HttpPatch("{permissionId:guid}/activate")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActivateAccessAsync([FromRoute] Guid permissionId, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new ActivateRoomAccessCommand(permissionId), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }

    // TODO: restringir este endpoint a usuários com Role == UserRole.Admin quando a autenticação for implementada.
    [HttpPatch("{permissionId:guid}/deactivate")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeactivateAccessAsync([FromRoute] Guid permissionId, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new DeactivateRoomAccessCommand(permissionId), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }
}