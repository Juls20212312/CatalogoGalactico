
using CatalogoGalactico.Dtos;

namespace CatalogoGalactico.Services;

public enum ResultadoAsignacionArma
{
    Asignada,
    ArmaNoExiste,
    PersonajeNoExiste,
    YaAsignada,
    RequiereFuerza
}

public enum ResultadoDesasignacionArma
{
    Desasignada,
    ArmaNoExiste,
    PersonajeNoExiste,
    NoEstabaAsignada
}

public enum ResultadoCambioArma
{
    Correcto,
    NoEncontrada,
    Conflicto
}

public interface IArmaService
{
    Task<List<ArmaDto>> ObtenerTodasAsync();

    Task<ArmaDto?> ObtenerPorIdAsync(int id);

    Task<ArmaDto> CrearAsync(ArmaCrearDto dto);
    Task<List<ArmaDto>?> ObtenerPorPersonajeAsync(int personajeId);
    Task<ResultadoCambioArma> ActualizarAsync(
        int id,
        ArmaCrearDto dto
    );

    Task<ResultadoCambioArma> EliminarAsync(int id);

    Task<ResultadoAsignacionArma> AsignarAsync(
        int armaId,
        int personajeId
    );

    Task<ResultadoDesasignacionArma> DesasignarAsync(
        int armaId,
        int personajeId
    );
}
