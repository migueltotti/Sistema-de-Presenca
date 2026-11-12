using SistemaPresenca.Application.Constants;
using SistemaPresenca.Domain.Entities;
using System.Text.Json;

namespace SistemaPresenca.Application.Mappers;

public static class SessionMappers
{
    public static string ToEndSessionEvent(this Session session)
    {
        var @event = new
        {
            SessionEvent = "EVT_INICIAR_REGISTRO_AULA",
            SessionId = session.Id,
            ProfessorTagIds = session
                .Subject
                .Professors
                .Where(x => !string.IsNullOrEmpty(x.TagId))
                .Select(x => x.TagId)
        };

        return JsonSerializer.Serialize(@event, SistemaPresencaConstants.SerializerOptions);
    }
}
