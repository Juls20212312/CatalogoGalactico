namespace CatalogoGalactico.Models;

public class Arma
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public int BonificacionPoder { get; set; }

    public bool RequiereFuerzaSensitivo { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public List<Personaje> Personajes { get; set; } = new();
}