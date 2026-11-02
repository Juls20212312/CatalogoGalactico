
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonajesController : ControllerBase
{
    private readonly IPersonajeService _service;

    public PersonajesController(IPersonajeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<PersonajeDto>>> ObtenerTodos(
        [FromQuery] Faccion? faccion,
        [FromQuery] bool? fuerzaSensitivo)
    {
        var personajes = await _service.ObtenerTodosAsync(
            faccion,
            fuerzaSensitivo
        );

        return Ok(personajes);
    }

    [HttpGet("ranking")]
    public async Task<ActionResult<List<object>>> ObtenerRanking(
        [FromQuery] string? por)
    {
        if (!string.IsNullOrWhiteSpace(por) &&
            !por.Equals("poder", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                mensaje = "El parámetro 'por' debe ser 'poder'."
            });
        }

        var ranking = await _service.ObtenerRankingPorPoderAsync();

        return Ok(ranking);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonajeDto>> ObtenerPorId(int id)
    {
        var personaje = await _service.ObtenerPorIdAsync(id);

        if (personaje is null)
        {
            return NotFound(new
            {
                mensaje = "Personaje no encontrado."
            });
        }

        return Ok(personaje);
    }

[HttpGet("{id:int}/poder")]
public async Task<IActionResult> ObtenerPoder(int id)
{
    var resultado = await _service.ObtenerPoderAsync(id);

    if (!resultado.ExistePersonaje)
    {
        return NotFound(new
        {
            mensaje = "El personaje no existe."
        });
    }

    if (resultado.Poder == null)
    {
        return Conflict(new
        {
            mensaje = "El personaje no tiene una carta registrada."
        });
    }

    return Ok(resultado.Poder);
}

    [HttpGet("{id:int}/eventos")]
    public async Task<ActionResult<List<EventoDto>>> ObtenerEventos(int id)
    {
        var eventos = await _service.ObtenerEventosAsync(id);

        if (eventos is null)
        {
            return NotFound(new
            {
                mensaje = "Personaje no encontrado."
            });
        }

        return Ok(eventos);
    }

    [HttpPost]
    public async Task<ActionResult<PersonajeDto>> Crear(
        [FromBody] PersonajeCrearDto dto)
    {
        var personaje = await _service.CrearAsync(dto);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = personaje.Id },
            personaje
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] PersonajeCrearDto dto)
    {
        var resultado = await _service.ActualizarAsync(id, dto);

        return resultado switch
        {
            ResultadoActualizarPersonaje.Actualizado => NoContent(),

            ResultadoActualizarPersonaje.NoEncontrado => NotFound(new
            {
                mensaje = "Personaje no encontrado."
            }),

            ResultadoActualizarPersonaje.MuerteRegistrada => Conflict(new
            {
                mensaje = "No se puede modificar el estado porque existe una muerte registrada."
            }),
            ResultadoActualizarPersonaje.ArmaRequiereFuerza => Conflict(new
            {
            mensaje = "No se puede quitar la sensibilidad a la Fuerza mientras el personaje tenga armas restringidas."
            }),
            _ => BadRequest()
        };
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _service.EliminarAsync(id);

        return resultado switch
        {
            ResultadoEliminarPersonaje.Eliminado => NoContent(),

            ResultadoEliminarPersonaje.NoEncontrado => NotFound(new
            {
                mensaje = "Personaje no encontrado."
            }),

            ResultadoEliminarPersonaje.TieneCarta => Conflict(new
            {
                mensaje = "No se puede eliminar el personaje porque tiene una carta asociada."
            }),

            ResultadoEliminarPersonaje.TieneEventos => Conflict(new
            {
                mensaje = "No se puede eliminar el personaje porque participa en eventos."
            }),
            ResultadoEliminarPersonaje.TieneArmas => Conflict(new
            {
                mensaje = "No se puede eliminar el personaje porque tiene armas asignadas."
            }),
            _ => BadRequest()
        };
    }
}
