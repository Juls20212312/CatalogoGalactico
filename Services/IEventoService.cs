using CatalogoGalactico.Dtos;

namespace CatalogoGalactico.Services;

public interface IEventoService
{
    Task<List<EventoDto>> ObtenerTodosAsync();

    Task<EventoDto?> ObtenerPorIdAsync(int id);

    Task<(EventoDto? Evento, string? Error)> CrearAsync(EventoCrearDto dto);

    Task<(bool Actualizado, string? Error)> ActualizarAsync(
        int id,
        EventoCrearDto dto
    );

    Task<bool> EliminarAsync(int id);

    Task<List<EventoDto>?> ObtenerEventosDePersonajeAsync(int personajeId);

    Task<object?> ObtenerMvpAsync(int eventoId);

    Task<(object? Resultado, string? Error)> SimularAsync(int eventoId);
}