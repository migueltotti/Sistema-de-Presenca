using System.Text.Json.Serialization;

namespace SistemaPresenca.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoomBookingStatus
{
    Scheduled,
    Fulfilled,
    Canceled,
    Expired
}