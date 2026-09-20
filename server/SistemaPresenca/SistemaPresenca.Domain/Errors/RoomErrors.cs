namespace SistemaPresenca.Domain.Errors;

public static class RoomErrors
{
    public static Error NameAlreadyExists => new(
        "Room.NameAlreadyExists",
        "A room with this name already exists.");

    public static Error MicrocontrollerIdAlreadyExists => new(
        "Room.MicrocontrollerIdAlreadyExists",
        "A room with this microcontroller id already exists.");

    public static Error NotFound => new(
        "Room.NotFound",
        "Room not found.");
}