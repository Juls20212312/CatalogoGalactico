using CatalogoGalactico.Dtos;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Crear(
        [FromBody] UsuarioCrearDto dto)
    {
        var usuario = await _service.CrearAsync(dto);

        if (usuario is null)
        {
            return Conflict(new
            {
                mensaje = "Ya existe un usuario con ese correo electrónico."
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            usuario
        );
    }
}