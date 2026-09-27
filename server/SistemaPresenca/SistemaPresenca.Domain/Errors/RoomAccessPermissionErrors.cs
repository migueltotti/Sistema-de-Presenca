using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Domain.Errors;

public static class RoomAccessPermissionErrors
{
    public static Error AlreadyExists => new(
        "RoomAccessPermission.AlreadyExists",
        "An access permission already exists for this user and room.");

    public static Error NotFound => new(
        "RoomAccessPermission.NotFound",
        "Room access permission not found.");

    public static Error UserNotFound => new(
        "RoomAccessPermission.UserNotFound",
        "User not found.");

    public static Error RoomNotFound => new(
        "RoomAccessPermission.RoomNotFound",
        "Room not found.");

    public static Error RoomInactive => new(
        "RoomAccessPermission.RoomInactive",
        "The room is inactive.");

    public static Error PermissionInactive => new(
        "RoomAccessPermission.PermissionInactive",
        "The user does not have an active permission for this room.");
}