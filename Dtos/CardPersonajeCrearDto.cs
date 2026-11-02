using System.ComponentModel.DataAnnotations;

namespace CatalogoGalactico.Dtos;

public class CardPersonajeCrearDto
{
    [Range(1, int.MaxValue)]
    public int PersonajeId { get; set; }

    [Range(0, int.MaxValue)]
    public int Poder { get; set; }

    [Required]
    [StringLength(200)]
    public string HabilidadEspecial { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Arma { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int NivelPeligrosidad { get; set; }

    [StringLength(500)]
    public string ImagenUrl { get; set; } = string.Empty;
}