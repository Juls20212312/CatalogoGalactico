using CatalogoGalactico.Dtos;

namespace CatalogoGalactico.Services;

public enum ResultadoOperacionCarta
{
    Correcto,
    NoEncontrada,
    PersonajeNoExiste,
    PersonajeYaTieneCarta,
    PoderInvalido
}

public interface ICardService
{
    Task<List<CardPersonajeDto>> ObtenerTodosAsync();

    Task<CardPersonajeDto?> ObtenerPorIdAsync(int id);

    Task<CardPersonajeDto?> ObtenerPorPersonajeIdAsync(int personajeId);

    Task<(ResultadoOperacionCarta Resultado, CardPersonajeDto? Carta)>
        CrearAsync(CardPersonajeCrearDto dto);

    Task<ResultadoOperacionCarta> ActualizarAsync(
        int id,
        CardPersonajeCrearDto dto
    );

    Task<bool> EliminarAsync(int id);
}