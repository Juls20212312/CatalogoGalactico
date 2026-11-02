using CatalogoGalactico.Dtos;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArmasController : ControllerBase
{
    private readonly IArmaService _armaService;

    public ArmasController(IArmaService armaService)
    {
        _armaService = armaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ArmaDto>>> ObtenerTodas()
    {
        var armas = await _armaService.ObtenerTodasAsync();

        return Ok(armas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ArmaDto>> ObtenerPorId(int id)
    {
        var arma = await _armaService.ObtenerPorIdAsync(id);

        if (arma == null)
        {
            return NotFound(new
            {
                mensaje = "El arma no existe."
            });
        }

        return Ok(arma);
    }

    [HttpPost]
    public async Task<ActionResult<ArmaDto>> Crear(ArmaCrearDto dto)
    {
        var arma = await _armaService.CrearAsync(dto);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = arma.Id },
            arma
        );
    }

    [HttpPut("{id:int}")]
    
public async Task<IActionResult> Actualizar(
    int id,
    ArmaCrearDto dto)
{
    var resultado = await _armaService.ActualizarAsync(id, dto);

    return resultado switch
    {
        ResultadoCambioArma.Correcto => NoContent(),

        ResultadoCambioArma.NoEncontrada =>
            NotFound(new
            {
                mensaje = "El arma no existe."
            }),

        ResultadoCambioArma.Conflicto =>
            Conflict(new
            {
                mensaje = "El arma tiene asignaciones incompatibles con el nuevo requisito de Fuerza."
            }),

        _ => BadRequest()
    };
}


    [HttpDelete("{id:int}")]
    
public async Task<IActionResult> Eliminar(int id)
{
    var resultado = await _armaService.EliminarAsync(id);

    return resultado switch
    {
        ResultadoCambioArma.Correcto => NoContent(),

        ResultadoCambioArma.NoEncontrada =>
            NotFound(new
            {
                mensaje = "El arma no existe."
            }),

        ResultadoCambioArma.Conflicto =>
            Conflict(new
            {
                mensaje = "No se puede eliminar un arma que tiene personajes asignados."
            }),

        _ => BadRequest()
    };
}


    [HttpPost("{armaId:int}/asignar/{personajeId:int}")]
    public async Task<IActionResult> Asignar(
        int armaId,
        int personajeId)
    {
        var resultado = await _armaService.AsignarAsync(
            armaId,
            personajeId
        );

        return resultado switch
        {
            ResultadoAsignacionArma.Asignada => NoContent(),

            ResultadoAsignacionArma.ArmaNoExiste =>
                NotFound(new { mensaje = "El arma no existe." }),

            ResultadoAsignacionArma.PersonajeNoExiste =>
                NotFound(new { mensaje = "El personaje no existe." }),

            ResultadoAsignacionArma.YaAsignada =>
                Conflict(new { mensaje = "El arma ya está asignada a este personaje." }),

            ResultadoAsignacionArma.RequiereFuerza =>
                Conflict(new { mensaje = "Este arma requiere que el personaje sea sensible a la Fuerza." }),

            _ => BadRequest()
        };
    }

    [HttpDelete("{armaId:int}/asignar/{personajeId:int}")]
    public async Task<IActionResult> Desasignar(
        int armaId,
        int personajeId)
    {
        var resultado = await _armaService.DesasignarAsync(
            armaId,
            personajeId
        );

        return resultado switch
        {
            ResultadoDesasignacionArma.Desasignada => NoContent(),

            ResultadoDesasignacionArma.ArmaNoExiste =>
                NotFound(new { mensaje = "El arma no existe." }),

            ResultadoDesasignacionArma.PersonajeNoExiste =>
                NotFound(new { mensaje = "El personaje no existe." }),

            ResultadoDesasignacionArma.NoEstabaAsignada =>
                Conflict(new { mensaje = "El arma no estaba asignada a este personaje." }),

            _ => BadRequest()
        };
    }
}