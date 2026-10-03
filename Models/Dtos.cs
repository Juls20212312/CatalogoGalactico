namespace CatalogoGalactico.Models;

public record PersonajeRequest(
    string Nombre,
    string Especie,
    Faccion Faccion,
    string Afiliacion,
    EstadoPersonaje Estado,
    bool FuerzaSensitivo
);

public record CardRequest(
    int PersonajeId,
    int Poder,
    string HabilidadEspecial,
    string Arma,
    int NivelPeligrosidad,
    string ImagenUrl
);

public record EventoRequest(
    string Nombre,
    int Fecha,
    string Ubicacion,
    string Descripcion,
    List<int> ParticipantesIds,
    int? GanadorId,
    string? Resultado,
    List<int> MuertosIds
);