using System.ComponentModel.DataAnnotations;

namespace CatalogoGalactico.Dtos;

public class EventoCrearDto
{
    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    public int Fecha { get; set; }

    [Required]
    [StringLength(150)]
    public string Ubicacion { get; set; } = string.Empty;

    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [MinLength(2, ErrorMessage = "Debe haber al menos dos participantes")]
    public List<int> ParticipantesIds { get; set; } = new();

    public int? GanadorId { get; set; }

    [StringLength(500)]
    public string? Resultado { get; set; }

    public List<int> MuertosIds { get; set; } = new();
}