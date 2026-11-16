using SistemaPresenca.Application.Constants;
using SistemaPresenca.Application.Requests.Session;
using SistemaPresenca.Domain.Entities;
using System.Text.Json;

namespace SistemaPresenca.Application.Mappers;

public static class SessionMappers
{
    public static Session ToEntity(this CreateSessionRequest request, Guid createdById)
    {
        return new Session(
            request.Date,
            request.ScheduledStartTime,
            request.ScheduledEndTime,
            request.TotalLessons,
            request.SubjectId,
            request.RoomId,
            createdById);
    }

    public static string ToEndSessionEvent(this Session session)
    {
        var @event = new
        {
            SessionEvent = "EVT_FINALIZAR_REGISTRO_AULA",
            SessionId = session.Id,
            ProfessorTagIds = session
                .Subject
                .Professors
                .Where(x => !string.IsNullOrEmpty(x.TagId))
                .Select(x => x.TagId)
        };

        return JsonSerializer.Serialize(@event, SistemaPresencaConstants.SerializerOptions);
    }

    public static string ToStartSessionEvent(this Session session)
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
