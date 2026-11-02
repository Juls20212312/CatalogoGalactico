
using System.ComponentModel.DataAnnotations;

namespace CatalogoGalactico.Dtos;

public class ArmaAsignarDto
{
    [Range(1, int.MaxValue)]
    public int ArmaId { get; set; }
}
