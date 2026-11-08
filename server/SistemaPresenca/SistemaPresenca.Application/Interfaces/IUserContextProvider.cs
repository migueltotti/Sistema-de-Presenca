namespace SistemaPresenca.Application.Interfaces;

public interface IUserContextProvider
{
    public Guid UserId { get; }
    public string UserEmail { get; }
    void SetUserId(Guid? id);
    void SetUserEmail(string? email);
}
