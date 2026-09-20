namespace SistemaPresenca.Domain.Entities;

public class Room
{
	public Guid Id { get; set; }

	public string Nome { get; set; } = string.Empty;

	public string Localizacao { get; set; } = string.Empty;

	public string IdentificadorMicrocontrolador { get; set; } = string.Empty;

	public RoomStatus Status { get; set; }

	public DateTime DataCadastro { get; set; }

	public Guid AdministradorResponsavelId { get; set; }
}

public enum RoomStatus
{
	Ativa,
	Inativa
}
