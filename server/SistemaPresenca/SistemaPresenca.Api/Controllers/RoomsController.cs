using AspNetCore.Authentication.ApiKey;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using SistemaPresenca.Application.Requests.Room;
using SistemaPresenca.Application.Requests.RoomAccessPermissions;
using SistemaPresenca.Application.Requests.Rooms;
using SistemaPresenca.Application.Responses.Rooms;
using SistemaPresenca.Application.UseCases.Commands.Rooms;
using SistemaPresenca.Application.UseCases.Commands.Rooms.CloseRoom;
using SistemaPresenca.Application.UseCases.Commands.Rooms.OpenRoom;
using SistemaPresenca.Application.UseCases.Queries.Rooms;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Api.Controllers;

[ApiController]
[Route("api/v1/rooms")]
public class RoomsController(ICommandMediator commandMediator, IQueryMediator queryMediator) : ControllerBase
{
    [HttpGet]
    [Authorize("AdminOrProfessor")]
    [ProducesResponseType(typeof(IEnumerable<GetRoomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoomsAsync([FromQuery] GetRoomsRequest request, CancellationToken cancellationToken)
    {
        var result = await queryMediator.QueryAsync(new GetRoomsQuery(request), cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize("Admin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRoomAsync([FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new CreateRoomCommand(request), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Created();
    }

    [HttpPatch("{id:guid}")]
    [Authorize("Admin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [Consumes("application/json-patch+json")]
    public async Task<IActionResult> UpdateRoomAsync([FromRoute] Guid id, [FromBody] JsonPatchDocument<UpdateRoomRequest> request, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new UpdateRoomCommand(id, request), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize("Admin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRoomAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new DeleteRoomCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }

    [HttpPost("{roomId:guid}/open")]
    [Authorize("AdminOrProfessor")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> OpenRoomAsync(
        [FromRoute] Guid roomId,
        [FromBody] RoomAccessRequest request,
        CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new OpenRoomCommand(roomId, request.UserId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("{roomId:guid}/close")]
    [Authorize("AdminOrProfessor")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CloseRoomAsync(
        [FromRoute] Guid roomId,
        [FromBody] RoomAccessRequest request,
        CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new CloseRoomCommand(roomId, request.UserId), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("/microcontroller/{microcontrollerId}/close/tag-id/{tagId}")]
    [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)] // TODO: Trocar para ApiKey schema
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CloseRoomByTagIdAsync(
        [FromRoute] string microcontrollerId,
        [FromRoute] string tagId,
        [FromBody] CloseRoomSessionRequest sessionRequest,
        CancellationToken cancellationToken)
    {
        var result = await commandMediator.SendAsync(new CloseRoomByTagIdCommand(microcontrollerId, tagId, sessionRequest), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}