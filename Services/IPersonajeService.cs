using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public enum ResultadoActualizarPersonaje
{
    Actualizado,
    NoEncontrado,
    MuerteRegistrada,
    ArmaRequiereFuerza
}

public enum ResultadoEliminarPersonaje
{
    Eliminado,
    NoEncontrado,
    TieneCarta,
    TieneEventos,
    TieneArmas
}

public interface IPersonajeService
{
    Task<List<PersonajeDto>> ObtenerTodosAsync(
        Faccion? faccion,
        bool? fuerzaSensitivo);

    Task<PersonajeDto?> ObtenerPorIdAsync(int id);

    Task<PersonajeDto> CrearAsync(PersonajeCrearDto dto);

    Task<ResultadoActualizarPersonaje> ActualizarAsync(
        int id,
        PersonajeCrearDto dto);

    Task<ResultadoEliminarPersonaje> EliminarAsync(int id);

    Task<List<EventoDto>?> ObtenerEventosAsync(int personajeId);

    Task<List<object>> ObtenerRankingPorPoderAsync();
    Task<(bool ExistePersonaje, object? Poder)> ObtenerPoderAsync(int id);
}