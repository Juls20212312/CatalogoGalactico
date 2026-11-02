namespace CatalogoGalactico.Dtos;

public class EventoDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int Fecha { get; set; }

    public string Ubicacion { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public List<int> ParticipantesIds { get; set; } = new();

    public int? GanadorId { get; set; }

    public string? Resultado { get; set; }

    public List<int> MuertosIds { get; set; } = new();
}