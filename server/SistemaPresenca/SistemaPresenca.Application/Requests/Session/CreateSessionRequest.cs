namespace SistemaPresenca.Application.Requests.Session;

public record CreateSessionRequest(
    DateOnly Date,
    TimeOnly ScheduledStartTime,
    TimeOnly ScheduledEndTime,
    Guid SubjectId,
    Guid RoomId,
    int TotalLessons
    // TODO: Inserir flag se quer criar o agendamento da sala com inicio automático, isso pode ser alterado depois
);
