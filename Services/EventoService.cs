using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class EventoService : IEventoService
{
    private readonly AppDbContext _db;

    public EventoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventoDto>> ObtenerTodosAsync()
    {
        var eventos = await _db.Eventos
            .AsNoTracking()
            .Include(e => e.Participaciones)
            .OrderBy(e => e.Fecha)
            .ToListAsync();

        return eventos.Select(ConvertirDto).ToList();
    }

    public async Task<EventoDto?> ObtenerPorIdAsync(int id)
    {
        var evento = await _db.Eventos
            .AsNoTracking()
            .Include(e => e.Participaciones)
            .FirstOrDefaultAsync(e => e.Id == id);

        return evento == null ? null : ConvertirDto(evento);
    }

    public async Task<(EventoDto? Evento, string? Error)> CrearAsync(
        EventoCrearDto dto)
    {
        string? error = await ValidarEventoAsync(dto);

        if (error != null)
        {
            return (null, error);
        }

        await using var transaccion =
            await _db.Database.BeginTransactionAsync();

        var evento = new Evento
        {
            Nombre = dto.Nombre,
            Fecha = dto.Fecha,
            Ubicacion = dto.Ubicacion,
            Descripcion = dto.Descripcion,
            GanadorId = dto.GanadorId,
            Resultado = dto.Resultado
        };

        _db.Eventos.Add(evento);

        await _db.SaveChangesAsync();

        var muertos = dto.MuertosIds ?? new List<int>();

        foreach (int personajeId in dto.ParticipantesIds)
        {
            _db.ParticipacionesEventos.Add(new ParticipacionEvento
            {
                EventoId = evento.Id,
                PersonajeId = personajeId,
                MurioEnEvento = muertos.Contains(personajeId)
            });
        }
        await _db.SaveChangesAsync();
        await RecalcularEstadosAsync(dto.ParticipantesIds);

        await _db.SaveChangesAsync();
        await transaccion.CommitAsync();

        await _db.Entry(evento)
            .Collection(e => e.Participaciones)
            .LoadAsync();

        return (ConvertirDto(evento), null);
    }

    public async Task<(bool Actualizado, string? Error)> ActualizarAsync(
        int id,
        EventoCrearDto dto)
    {
        var evento = await _db.Eventos
            .Include(e => e.Participaciones)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evento == null)
        {
            return (false, "El evento no existe.");
        }

        string? error = await ValidarEventoAsync(dto, id);

        if (error != null)
        {
            return (false, error);
        }

        var personajesAfectados = evento.Participaciones
            .Select(p => p.PersonajeId)
            .Union(dto.ParticipantesIds)
            .Distinct()
            .ToList();
        var muertosAntes = evento.Participaciones
            .Where(p => p.MurioEnEvento)
            .Select(p => p.PersonajeId)
            .ToList();
        await using var transaccion =
            await _db.Database.BeginTransactionAsync();

        _db.ParticipacionesEventos.RemoveRange(evento.Participaciones);

        await _db.SaveChangesAsync();

        evento.Nombre = dto.Nombre;
        evento.Fecha = dto.Fecha;
        evento.Ubicacion = dto.Ubicacion;
        evento.Descripcion = dto.Descripcion;
        evento.GanadorId = dto.GanadorId;
        evento.Resultado = dto.Resultado;
        var muertos = dto.MuertosIds ?? new List<int>();

        foreach (int personajeId in dto.ParticipantesIds)
        {
            _db.ParticipacionesEventos.Add(new ParticipacionEvento
            {
                EventoId = evento.Id,
                PersonajeId = personajeId,
                MurioEnEvento = muertos.Contains(personajeId)
            });
        }

        await _db.SaveChangesAsync();

        await RecalcularEstadosAsync(personajesAfectados, muertosAntes);
        await transaccion.CommitAsync();

        return (true, null);
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var evento = await _db.Eventos
            .Include(e => e.Participaciones)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evento == null)
        {
            return false;
        }

        var personajesAfectados = evento.Participaciones
            .Select(p => p.PersonajeId)
            .Distinct()
            .ToList();
        var muertosAntes = evento.Participaciones
            .Where(p => p.MurioEnEvento)
            .Select(p => p.PersonajeId)
            .ToList();
        await using var transaccion =
            await _db.Database.BeginTransactionAsync();

        _db.ParticipacionesEventos.RemoveRange(evento.Participaciones);
        _db.Eventos.Remove(evento);

        await _db.SaveChangesAsync();

        await RecalcularEstadosAsync(personajesAfectados, muertosAntes);

        await _db.SaveChangesAsync();
        await transaccion.CommitAsync();

        return true;
    }

    public async Task<List<EventoDto>?> ObtenerEventosDePersonajeAsync(
        int personajeId)
    {
        bool existePersonaje = await _db.Personajes
            .AnyAsync(p => p.Id == personajeId);

        if (!existePersonaje)
        {
            return null;
        }

        var eventos = await _db.Eventos
            .AsNoTracking()
            .Include(e => e.Participaciones)
            .Where(e => e.Participaciones.Any(
                p => p.PersonajeId == personajeId))
            .OrderBy(e => e.Fecha)
            .ToListAsync();

        return eventos.Select(ConvertirDto).ToList();
    }

    
public async Task<object?> ObtenerMvpAsync(int eventoId)
{
    var evento = await _db.Eventos
        .AsNoTracking()
        .Include(e => e.Participaciones)
            .ThenInclude(p => p.Personaje)
                .ThenInclude(p => p.CardPersonaje)
        .Include(e => e.Participaciones)
            .ThenInclude(p => p.Personaje)
                .ThenInclude(p => p.Armas)
        .AsSplitQuery()
        .FirstOrDefaultAsync(e => e.Id == eventoId);

    if (evento == null)
    {
        return null;
    }

    if (evento.Participaciones.Count == 0 ||
        evento.Participaciones.Any(p =>
            p.Personaje.CardPersonaje == null))
    {
        return null;
    }

    var mvp = evento.Participaciones
        .Select(p =>
        {
            var personaje = p.Personaje;

            int poderBase = personaje.CardPersonaje!.Poder;

            int bonificacionArmas = personaje.Armas.Sum(
                a => a.BonificacionPoder);

            return new
            {
                PersonajeId = personaje.Id,
                Nombre = personaje.Nombre,
                PoderBase = poderBase,
                BonificacionArmas = bonificacionArmas,
                PoderEfectivo = poderBase + bonificacionArmas
            };
        })
        .OrderByDescending(p => p.PoderEfectivo)
        .ThenBy(p => p.PersonajeId)
        .First();

    return mvp;
}


    
public async Task<(object? Resultado, string? Error)> SimularAsync(
    int eventoId)
{
    var evento = await _db.Eventos
        .AsNoTracking()
        .Include(e => e.Participaciones)
            .ThenInclude(p => p.Personaje)
                .ThenInclude(p => p.CardPersonaje)
        .Include(e => e.Participaciones)
            .ThenInclude(p => p.Personaje)
                .ThenInclude(p => p.Armas)
        .AsSplitQuery()
        .FirstOrDefaultAsync(e => e.Id == eventoId);

    if (evento == null)
    {
        return (null, "El evento no existe.");
    }

    if (evento.Participaciones.Count < 2)
    {
        return (null,
            "Se necesitan al menos dos participantes.");
    }

    if (evento.Participaciones.Any(p =>
        p.Personaje.CardPersonaje == null))
    {
        return (null,
            "Todos los participantes deben tener una carta registrada.");
    }

    var participantes = evento.Participaciones
        .Select(p =>
        {
            var personaje = p.Personaje;

            int poderBase = personaje.CardPersonaje!.Poder;

            int bonificacionArmas = personaje.Armas.Sum(
                a => a.BonificacionPoder);

            return new
            {
                PersonajeId = personaje.Id,
                Nombre = personaje.Nombre,
                Faccion = personaje.Faccion,
                PoderBase = poderBase,
                BonificacionArmas = bonificacionArmas,
                PoderEfectivo = poderBase + bonificacionArmas
            };
        })
        .ToList();

    var facciones = participantes
        .GroupBy(p => p.Faccion)
        .ToList();

    if (facciones.Count < 2)
    {
        return (null,
            "Se necesitan al menos dos facciones para realizar la simulación.");
    }

    var resultados = facciones.Select(grupo =>
    {
        int poderBase = grupo.Sum(p => p.PoderBase);

        int bonificacionArmas = grupo.Sum(
            p => p.BonificacionArmas);

        int poderEfectivo = poderBase + bonificacionArmas;

        double factor = 0.90 +
            (Random.Shared.NextDouble() * 0.20);

        double poderFinal = poderEfectivo * factor;

        return new
        {
            Faccion = grupo.Key,
            PoderBase = poderBase,
            BonificacionArmas = bonificacionArmas,
            PoderEfectivo = poderEfectivo,
            Factor = Math.Round(factor, 2),
            PoderFinal = Math.Round(poderFinal, 2)
        };
    }).ToList();

    var ganadora = resultados
        .OrderByDescending(r => r.PoderFinal)
        .First();

    bool empate = resultados.Count(
        r => r.PoderFinal == ganadora.PoderFinal) > 1;

    var personajeDestacado = empate
        ? null
        : participantes
            .Where(p => p.Faccion == ganadora.Faccion)
            .OrderByDescending(p => p.PoderEfectivo)
            .FirstOrDefault();

    var resultado = new
    {
        EventoId = evento.Id,
        NombreEvento = evento.Nombre,
        Participantes = participantes,
        FaccionGanadora = empate
            ? "Empate"
            : ganadora.Faccion.ToString(),
        PersonajeDestacado = personajeDestacado?.Nombre,
        PoderesPorFaccion = resultados,
        Motivo = empate
            ? "Las facciones empataron en el poder final calculado."
            : $"La facción {ganadora.Faccion} obtuvo el mayor poder después de aplicar las bonificaciones y el factor aleatorio."
    };

    return (resultado, null);
}


    private async Task<string?> ValidarEventoAsync(
        EventoCrearDto dto,
        int? eventoActualId = null)
    {
        var participantes = dto.ParticipantesIds ?? new List<int>();
        var muertos = dto.MuertosIds ?? new List<int>();

        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return "El nombre del evento es obligatorio.";
        }

        if (participantes.Count < 2)
        {
            return "El evento necesita al menos dos participantes.";
        }

        if (participantes.Distinct().Count() != participantes.Count)
        {
            return "No se permiten participantes repetidos.";
        }

        if (muertos.Distinct().Count() != muertos.Count)
        {
            return "No se permiten personajes repetidos en la lista de muertos.";
        }

        int personajesExistentes = await _db.Personajes
            .CountAsync(p => participantes.Contains(p.Id));

        if (personajesExistentes != participantes.Count)
        {
            return "Uno o más participantes no existen.";
        }

        if (muertos.Any(id => !participantes.Contains(id)))
        {
            return "Los personajes muertos deben formar parte del evento.";
        }

        if (dto.GanadorId.HasValue &&
            !participantes.Contains(dto.GanadorId.Value))
        {
            return "El ganador debe ser uno de los participantes.";
        }

        var muerteAnterior = await (
            from participacion in _db.ParticipacionesEventos
            join evento in _db.Eventos
                on participacion.EventoId equals evento.Id
            where participantes.Contains(participacion.PersonajeId)
                && participacion.MurioEnEvento
                && evento.Fecha <= dto.Fecha
                && (!eventoActualId.HasValue ||
                    evento.Id != eventoActualId.Value)
            select participacion
        ).AnyAsync();

        if (muerteAnterior)
        {
            return "Un personaje que ya murió no puede participar en un evento posterior.";
        }

        var participacionPosterior = await (
            from participacion in _db.ParticipacionesEventos
            join evento in _db.Eventos
                on participacion.EventoId equals evento.Id
            where muertos.Contains(participacion.PersonajeId)
                && evento.Fecha > dto.Fecha
                && (!eventoActualId.HasValue ||
                    evento.Id != eventoActualId.Value)
            select participacion
        ).AnyAsync();

        if (participacionPosterior)
        {
            return "Un personaje que muere en este evento no puede tener participaciones posteriores.";
        }

        return null;
    }

    
private async Task RecalcularEstadosAsync(
    List<int> personajesIds,
    List<int>? muertosAntes = null)
{
    var personajes = await _db.Personajes
        .Where(p => personajesIds.Contains(p.Id))
        .ToListAsync();

    foreach (var personaje in personajes)
    {
        bool tieneMuerteRegistrada = await _db.ParticipacionesEventos
            .AnyAsync(p =>
                p.PersonajeId == personaje.Id &&
                p.MurioEnEvento);

        if (tieneMuerteRegistrada)
        {
            personaje.Estado = EstadoPersonaje.Muerto;
        }
        else if (muertosAntes != null &&
                 muertosAntes.Contains(personaje.Id) &&
                 personaje.Estado == EstadoPersonaje.Muerto)
        {
            personaje.Estado = EstadoPersonaje.Vivo;
        }
    }
}


    private static EventoDto ConvertirDto(Evento evento)
    {
        return new EventoDto
        {
            Id = evento.Id,
            Nombre = evento.Nombre,
            Fecha = evento.Fecha,
            Ubicacion = evento.Ubicacion,
            Descripcion = evento.Descripcion,
            ParticipantesIds = evento.Participaciones
                .Select(p => p.PersonajeId)
                .ToList(),
            MuertosIds = evento.Participaciones
                .Where(p => p.MurioEnEvento)
                .Select(p => p.PersonajeId)
                .ToList(),
            GanadorId = evento.GanadorId,
            Resultado = evento.Resultado
        };
    }
}