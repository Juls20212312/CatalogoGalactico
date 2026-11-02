
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IEventoService _service;

    public EventosController(IEventoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<EventoDto>>> ObtenerTodos()
    {
        var eventos = await _service.ObtenerTodosAsync();

        return Ok(eventos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventoDto>> ObtenerPorId(int id)
    {
        var evento = await _service.ObtenerPorIdAsync(id);

        if (evento is null)
        {
            return NotFound(new
            {
                mensaje = "Evento no encontrado."
            });
        }

        return Ok(evento);
    }

    [HttpPost]
    public async Task<ActionResult<EventoDto>> Crear(
        [FromBody] EventoCrearDto dto)
    {
        var resultado = await _service.CrearAsync(dto);

        if (resultado.Evento is null)
        {
            return BadRequest(new
            {
                mensaje = resultado.Error
            });
        }

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = resultado.Evento.Id },
            resultado.Evento
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] EventoCrearDto dto)
    {
        var resultado = await _service.ActualizarAsync(id, dto);

        if (!resultado.Actualizado)
        {
            if (resultado.Error == "El evento no existe.")
            {
                return NotFound(new
                {
                    mensaje = "Evento no encontrado."
                });
            }

            return BadRequest(new
            {
                mensaje = resultado.Error
            });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _service.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje = "Evento no encontrado."
            });
        }

        return NoContent();
    }

    [HttpGet("{id:int}/mvp")]
    

public async Task<IActionResult> ObtenerMvp(int id)
{
    var mvp = await _service.ObtenerMvpAsync(id);

    if (mvp == null)
    {
        var evento = await _service.ObtenerPorIdAsync(id);

        if (evento == null)
        {
            return NotFound(new
            {
                mensaje = "El evento no existe."
            });
        }

        return Conflict(new
        {
            mensaje = "El evento no tiene participantes o faltan cartas para calcular el MVP."
        });
    }

    return Ok(mvp);
}


    [HttpPost("{id:int}/simular")]
    

public async Task<IActionResult> Simular(int id)
{
    var resultado = await _service.SimularAsync(id);

    if (resultado.Resultado == null)
    {
        if (resultado.Error == "El evento no existe.")
        {
            return NotFound(new
            {
                mensaje = resultado.Error
            });
        }

        if (resultado.Error ==
            "Todos los participantes deben tener una carta registrada.")
        {
            return Conflict(new
            {
                mensaje = resultado.Error
            });
        }

        return BadRequest(new
        {
            mensaje = resultado.Error
        });
    }

    return Ok(resultado.Resultado);
}

}
