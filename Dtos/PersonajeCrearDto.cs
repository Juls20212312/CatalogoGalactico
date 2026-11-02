using System.ComponentModel.DataAnnotations;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Dtos;

public class PersonajeCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especie es obligatoria")]
    [StringLength(80)]
    public string Especie { get; set; } = string.Empty;

    [EnumDataType(typeof(Faccion))]
    public Faccion Faccion { get; set; }

    [Required(ErrorMessage = "La afiliación es obligatoria")]
    [StringLength(100)]
    public string Afiliacion { get; set; } = string.Empty;

    [EnumDataType(typeof(EstadoPersonaje))]
    public EstadoPersonaje Estado { get; set; }

    public bool FuerzaSensitivo { get; set; }
    public string UbicacionSecreta { get; set; } = string.Empty;
}