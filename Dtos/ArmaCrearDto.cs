using System.ComponentModel.DataAnnotations;

namespace CatalogoGalactico.Dtos;

public class ArmaCrearDto
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Range(0, 1000)]
    public int BonificacionPoder { get; set; }

    public bool RequiereFuerzaSensitivo { get; set; }

    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;
}