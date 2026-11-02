
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalactico.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartasController : ControllerBase
{
    private readonly ICardService _service;

    public CartasController(ICardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<CardPersonajeDto>>> ObtenerTodos()
    {
        var cartas = await _service.ObtenerTodosAsync();

        return Ok(cartas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CardPersonajeDto>> ObtenerPorId(int id)
    {
        var carta = await _service.ObtenerPorIdAsync(id);

        if (carta is null)
        {
            return NotFound(new
            {
                mensaje = "Carta no encontrada."
            });
        }

        return Ok(carta);
    }

    [HttpGet("personaje/{personajeId:int}")]
    public async Task<ActionResult<CardPersonajeDto>> ObtenerPorPersonaje(
        int personajeId)
    {
        var carta = await _service.ObtenerPorPersonajeIdAsync(personajeId);

        if (carta is null)
        {
            return NotFound(new
            {
                mensaje = "El personaje no existe o no tiene carta."
            });
        }

        return Ok(carta);
    }

    [HttpPost]
    public async Task<ActionResult<CardPersonajeDto>> Crear(
        [FromBody] CardPersonajeCrearDto dto)
    {
        var resultado = await _service.CrearAsync(dto);

        if (resultado.Carta is null)
        {
            return resultado.Resultado switch
            {
                ResultadoOperacionCarta.PersonajeNoExiste => NotFound(new
                {
                    mensaje = "El personaje indicado no existe."
                }),

                ResultadoOperacionCarta.PersonajeYaTieneCarta => Conflict(new
                {
                    mensaje = "El personaje ya tiene una carta."
                }),

                ResultadoOperacionCarta.PoderInvalido => BadRequest(new
                {
                    mensaje = "El poder no puede ser negativo."
                }),

                _ => BadRequest()
            };
        }

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = resultado.Carta.Id },
            resultado.Carta
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        [FromBody] CardPersonajeCrearDto dto)
    {
        var resultado = await _service.ActualizarAsync(id, dto);

        return resultado switch
        {
            ResultadoOperacionCarta.Correcto => NoContent(),

            ResultadoOperacionCarta.NoEncontrada => NotFound(new
            {
                mensaje = "Carta no encontrada."
            }),

            ResultadoOperacionCarta.PersonajeNoExiste => NotFound(new
            {
                mensaje = "El personaje indicado no existe."
            }),

            ResultadoOperacionCarta.PersonajeYaTieneCarta => Conflict(new
            {
                mensaje = "El personaje ya tiene otra carta."
            }),

            ResultadoOperacionCarta.PoderInvalido => BadRequest(new
            {
                mensaje = "El poder no puede ser negativo."
            }),

            _ => BadRequest()
        };
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _service.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound(new
            {
                mensaje = "Carta no encontrada."
            });
        }

        return NoContent();
    }
}
