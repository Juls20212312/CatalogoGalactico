using CatalogoGalactico.Dtos;

namespace CatalogoGalactico.Services;

public interface IUsuarioService
{
    Task<UsuarioDto?> CrearAsync(UsuarioCrearDto dto);
}