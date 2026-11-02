using CatalogoGalactico.Models;

namespace CatalogoGalactico.Dtos;

public class PersonajeDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Especie { get; set; } = string.Empty;

    public Faccion Faccion { get; set; }

    public string Afiliacion { get; set; } = string.Empty;

    public EstadoPersonaje Estado { get; set; }

    public bool FuerzaSensitivo { get; set; }
    public string UbicacionSecreta { get; set; } = string.Empty;
}