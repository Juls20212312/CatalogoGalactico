using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public class PersonajeService
{
    private readonly DataStore _data;

    public PersonajeService(DataStore data)
    {
        _data = data;
    }

    public IEnumerable<Personaje> ObtenerTodos(
        Faccion? faccion = null,
        bool? fuerzaSensitivo = null)
    {
        IEnumerable<Personaje> personajes = _data.Personajes;

        if (faccion.HasValue)
        {
            personajes = personajes.Where(p => p.Faccion == faccion.Value);
        }

        if (fuerzaSensitivo.HasValue)
        {
            personajes = personajes.Where(
                p => p.FuerzaSensitivo == fuerzaSensitivo.Value);
        }

        return personajes;
    }

    public Personaje? ObtenerPorId(int id)
    {
        return _data.Personajes.FirstOrDefault(p => p.Id == id);
    }

    public Personaje Crear(PersonajeRequest request)
    {
        int nuevoId = _data.Personajes.Count == 0
            ? 1
            : _data.Personajes.Max(p => p.Id) + 1;

        var personaje = new Personaje
        {
            Id = nuevoId,
            Nombre = request.Nombre,
            Especie = request.Especie,
            Faccion = request.Faccion,
            Afiliacion = request.Afiliacion,
            Estado = request.Estado,
            FuerzaSensitivo = request.FuerzaSensitivo,
            Image = request.Image
        };

        _data.Personajes.Add(personaje);

        return personaje;
    }

    public (bool Ok, string? Error, Personaje? Personaje) Actualizar(
    int id,
    PersonajeRequest request)
{
    var personaje = ObtenerPorId(id);

    if (personaje is null)
    {
        return (false, "NOT_FOUND", null);
    }

    bool tieneMuerteRegistrada = _data.Eventos
        .Any(e => e.MuertosIds.Contains(id));

    if (tieneMuerteRegistrada &&
        request.Estado != EstadoPersonaje.Muerto)
    {
        return (
            false,
            "El estado no puede cambiarse porque existe un evento que registra la muerte del personaje.",
            null);
    }

    personaje.Nombre = request.Nombre;
    personaje.Especie = request.Especie;
    personaje.Faccion = request.Faccion;
    personaje.Afiliacion = request.Afiliacion;
    personaje.Estado = request.Estado;
    personaje.FuerzaSensitivo = request.FuerzaSensitivo;
    personaje.Image = request.Image;
    return (true, null, personaje);
}

    public string? Eliminar(int id)
{
    var personaje = ObtenerPorId(id);

    if (personaje is null)
    {
        return "NOT_FOUND";
    }

    bool tieneCarta = _data.Cartas
        .Any(c => c.PersonajeId == id);

    if (tieneCarta)
    {
        return "El personaje no puede eliminarse porque tiene una carta asociada.";
    }

    bool participaEnEvento = _data.Eventos
        .Any(e => e.ParticipantesIds.Contains(id));

    if (participaEnEvento)
    {
        return "El personaje no puede eliminarse porque participa en eventos.";
    }

    _data.Personajes.Remove(personaje);

    return null;
}

    public IEnumerable<object> ObtenerRankingPorPoder()
    {
        return _data.Personajes
            .Select(personaje => new
            {
                Personaje = personaje,
                Carta = _data.Cartas.FirstOrDefault(
                    c => c.PersonajeId == personaje.Id)
            })
            .Where(x => x.Carta is not null)
            .OrderByDescending(x => x.Carta!.Poder)
            .Select(x => new
            {
                x.Personaje.Id,
                x.Personaje.Nombre,
                Poder = x.Carta!.Poder
            });
    }
    public object? ObtenerConCard(int id)
{
    var personaje = ObtenerPorId(id);

    if (personaje is null)
    {
        return null;
    }

    var carta = _data.Cartas
        .FirstOrDefault(c => c.PersonajeId == id);

    return new
    {
        personaje.Id,
        personaje.Nombre,
        personaje.Especie,
        personaje.Faccion,
        personaje.Afiliacion,
        personaje.Estado,
        personaje.FuerzaSensitivo,
        personaje.Image,

        Card = carta is null
            ? null
            : new
            {
                carta.Poder,
                Peligrosidad = carta.NivelPeligrosidad,
                carta.HabilidadEspecial,
                carta.Arma
            }
    };
}
    public IEnumerable<Evento> ObtenerEventos(int personajeId)
    {
        return _data.Eventos
            .Where(e => e.ParticipantesIds.Contains(personajeId));
    }
}