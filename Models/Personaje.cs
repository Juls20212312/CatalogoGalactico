namespace CatalogoGalactico.Models;

public class Personaje
{
    public int Id { get; set; }
    public string Image { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    public string Especie { get; set; } = string.Empty;

    public Faccion Faccion { get; set; }

    public string Afiliacion { get; set; } = string.Empty;

    public EstadoPersonaje Estado { get; set; }

    public bool FuerzaSensitivo { get; set; }
}