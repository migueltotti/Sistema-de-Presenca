using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.RoomAccessPermissions;
using SistemaPresenca.Application.UseCases.Commands.RoomAccessPermissions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/rooms/{roomId:guid}/access-permissions")]
public class RoomAccessPermissionsController(ICommandMediator commandMediator) : ControllerBase
{
    [HttpPost]
    [Authorize("Admin")]
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

    [HttpDelete("{permissionId:guid}")]
    [Authorize("Admin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RevokeAccessAsync(
        [FromRoute] Guid roomId,
        [FromRoute] Guid permissionId, 
        CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new RevokeRoomAccessCommand(roomId, permissionId), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }

    [HttpPatch("{permissionId:guid}/activate")]
    [Authorize("Admin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActivateAccessAsync(
        [FromRoute] Guid roomId,
        [FromRoute] Guid permissionId,
        CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new ActivateRoomAccessCommand(roomId, permissionId), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }

    [HttpPatch("{permissionId:guid}/deactivate")]
    [Authorize("Admin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeactivateAccessAsync(
        [FromRoute] Guid roomId,
        [FromRoute] Guid permissionId,
        CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new DeactivateRoomAccessCommand(roomId, permissionId), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }
}