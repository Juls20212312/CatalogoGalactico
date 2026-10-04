using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public class EventoService
{
    private readonly DataStore _data;

    public EventoService(DataStore data)
    {
        _data = data;
    }

    public IEnumerable<Evento> ObtenerTodos()
    {
        return _data.Eventos;
    }

    public Evento? ObtenerPorId(int id)
    {
        return _data.Eventos.FirstOrDefault(e => e.Id == id);
    }

    public string? ValidarEvento(EventoRequest request, int? eventoIdExcluir = null)
{
    if (string.IsNullOrWhiteSpace(request.Nombre))
    {
        return "El nombre del evento es obligatorio.";
    }

    if (request.ParticipantesIds is null ||
        request.ParticipantesIds.Count < 2)
    {
        return "El evento debe tener al menos dos participantes.";
    }

    if (request.ParticipantesIds.Distinct().Count() !=
        request.ParticipantesIds.Count)
    {
        return "No puede haber participantes repetidos.";
    }

    foreach (int personajeId in request.ParticipantesIds)
    {
        if (!_data.Personajes.Any(p => p.Id == personajeId))
        {
            return $"El personaje con ID {personajeId} no existe.";
        }
    }

    foreach (int personajeId in request.MuertosIds)
    {
        if (!request.ParticipantesIds.Contains(personajeId))
        {
            return $"El personaje muerto {personajeId} debe ser participante del evento.";
        }
    }

    if (request.GanadorId.HasValue &&
        !request.ParticipantesIds.Contains(request.GanadorId.Value))
    {
        return "El ganador debe formar parte de los participantes.";
    }

    var eventosAnteriores = _data.Eventos
        .Where(e =>
            e.Id != eventoIdExcluir &&
            e.Fecha < request.Fecha);

    foreach (int personajeId in request.ParticipantesIds)
    {
        bool murioAntes = eventosAnteriores.Any(
            e => e.MuertosIds.Contains(personajeId));

        if (murioAntes)
        {
            var personaje = _data.Personajes
                .First(p => p.Id == personajeId);

            return $"El personaje {personaje.Nombre} no puede participar porque murió en un evento anterior.";
        }
    }

    var muertos = (request.MuertosIds ?? new List<int>())
        .Distinct()
        .ToList();

    foreach (int personajeId in muertos)
    {
        bool tieneParticipacionPosterior = _data.Eventos.Any(e =>
            e.Id != eventoIdExcluir &&
            e.Fecha > request.Fecha &&
            e.ParticipantesIds.Contains(personajeId));

        if (tieneParticipacionPosterior)
        {
            var personaje = _data.Personajes
                .First(p => p.Id == personajeId);

            return $"El personaje {personaje.Nombre} no puede morir en este evento porque ya participa en un evento posterior a esta fecha.";
        }
    }

    return null;
}
    public Evento Crear(EventoRequest request)
    {
        int nuevoId = _data.Eventos.Count == 0
            ? 1
            : _data.Eventos.Max(e => e.Id) + 1;

        var evento = new Evento
        {
            Id = nuevoId,
            Nombre = request.Nombre,
            Fecha = request.Fecha,
            Ubicacion = request.Ubicacion,
            Descripcion = request.Descripcion,
            ParticipantesIds = request.ParticipantesIds.ToList(),
            GanadorId = request.GanadorId,
            Resultado = request.Resultado,
            MuertosIds = request.MuertosIds.ToList()
        };

        _data.Eventos.Add(evento);

        ActualizarEstadosPorMuertes(evento);

        return evento;
    }

    public bool Actualizar(int id, EventoRequest request)
    {
        var evento = ObtenerPorId(id);

        if (evento is null)
        {
            return false;
        }

        evento.Nombre = request.Nombre;
        evento.Fecha = request.Fecha;
        evento.Ubicacion = request.Ubicacion;
        evento.Descripcion = request.Descripcion;
        evento.ParticipantesIds = request.ParticipantesIds.ToList();
        evento.GanadorId = request.GanadorId;
        evento.Resultado = request.Resultado;
        evento.MuertosIds = request.MuertosIds.ToList();

        ActualizarEstadosGenerales();

        return true;
    }

    public bool Eliminar(int id)
    {
        var evento = ObtenerPorId(id);

        if (evento is null)
        {
            return false;
        }

        _data.Eventos.Remove(evento);

        ActualizarEstadosGenerales();

        return true;
    }

    public IEnumerable<Evento> ObtenerEventosDePersonaje(int personajeId)
    {
        return _data.Eventos
            .Where(e => e.ParticipantesIds.Contains(personajeId));
    }

    public object? ObtenerMvp(int eventoId)
    {
        var evento = ObtenerPorId(eventoId);

        if (evento is null)
        {
            return null;
        }

        var participantesConCarta = evento.ParticipantesIds
            .Select(personajeId => new
            {
                Personaje = _data.Personajes
                    .FirstOrDefault(p => p.Id == personajeId),

                Carta = _data.Cartas
                    .FirstOrDefault(c => c.PersonajeId == personajeId)
            })
            .Where(x => x.Personaje is not null && x.Carta is not null)
            .OrderByDescending(x => x.Carta!.Poder)
            .FirstOrDefault();

        if (participantesConCarta is null)
        {
            return null;
        }

        return new
        {
            EventoId = evento.Id,
            Evento = evento.Nombre,
            PersonajeId = participantesConCarta.Personaje!.Id,
            Personaje = participantesConCarta.Personaje.Nombre,
            Poder = participantesConCarta.Carta!.Poder,
            Criterio = "Mayor poder de carta entre los participantes."
        };
    }

    public object? Simular(int eventoId)
    {
        var evento = ObtenerPorId(eventoId);

        if (evento is null)
        {
            return null;
        }

        var datos = evento.ParticipantesIds
            .Select(personajeId => new
            {
                Personaje = _data.Personajes
                    .FirstOrDefault(p => p.Id == personajeId),

                Carta = _data.Cartas
                    .FirstOrDefault(c => c.PersonajeId == personajeId)
            })
            .ToList();

        if (datos.Any(x => x.Personaje is null || x.Carta is null))
        {
            throw new InvalidOperationException(
                "Todos los participantes deben tener personaje y carta.");
        }

        var bandos = datos
            .GroupBy(x => x.Personaje!.Faccion)
            .ToList();

        if (bandos.Count < 2)
        {
            throw new InvalidOperationException(
                "La simulación necesita participantes de al menos dos facciones.");
        }

        var resultados = bandos
            .Select(bando =>
            {
                int poderBase = bando.Sum(x => x.Carta!.Poder);

                double factor = 0.90 +
                    Random.Shared.NextDouble() * 0.20;

                double poderFinal = poderBase * factor;

                return new
                {
                    Faccion = bando.Key.ToString(),
                    PoderBase = poderBase,
                    FactorAleatorio = Math.Round(factor, 2),
                    PoderFinal = Math.Round(poderFinal, 2)
                };
            })
            .OrderByDescending(x => x.PoderFinal)
            .ToList();

        var ganador = resultados.First();

        var ganadorPersonaje = datos
            .Where(x => x.Personaje!.Faccion.ToString() == ganador.Faccion)
            .OrderByDescending(x => x.Carta!.Poder)
            .First();

        return new
        {
            EventoId = evento.Id,
            Evento = evento.Nombre,
            Resultado = $"La facción {ganador.Faccion} gana la simulación.",
            GanadorFaccion = ganador.Faccion,
            GanadorPersonaje = ganadorPersonaje.Personaje!.Nombre,
            TotalesPorBando = resultados,
            Criterio = "Se suma el poder de las cartas por facción y se aplica un factor aleatorio acotado entre 0.90 y 1.10."
        };
    }

    private void ActualizarEstadosPorMuertes(Evento evento)
    {
        foreach (int personajeId in evento.MuertosIds)
        {
            var personaje = _data.Personajes
                .FirstOrDefault(p => p.Id == personajeId);

            if (personaje is not null)
            {
                personaje.Estado = EstadoPersonaje.Muerto;
            }
        }
    }

    private void ActualizarEstadosGenerales()
    {
        foreach (var personaje in _data.Personajes)
        {
            bool murio = _data.Eventos.Any(
                e => e.MuertosIds.Contains(personaje.Id));

            personaje.Estado = murio
                ? EstadoPersonaje.Muerto
                : EstadoPersonaje.Vivo;
        }
    }
}