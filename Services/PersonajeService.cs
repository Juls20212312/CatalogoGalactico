
using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class PersonajeService : IPersonajeService
{
    private readonly AppDbContext _db;

    private readonly IProteccionService _proteccionService;
    public PersonajeService(
    AppDbContext db,
    IProteccionService proteccionService)
{
    _db = db;
    _proteccionService = proteccionService;
}

    public async Task<List<PersonajeDto>> ObtenerTodosAsync(
    Faccion? faccion,
    bool? fuerzaSensitivo)
{
    var consulta = _db.Personajes.AsNoTracking();

    if (faccion.HasValue)
        consulta = consulta.Where(p => p.Faccion == faccion.Value);

    if (fuerzaSensitivo.HasValue)
        consulta = consulta.Where(
            p => p.FuerzaSensitivo == fuerzaSensitivo.Value);

    var personajes = await consulta
        .OrderBy(p => p.Nombre)
        .ToListAsync();

    return personajes.Select(ConvertirDto).ToList();
}
    public async Task<PersonajeDto?> ObtenerPorIdAsync(int id)
{
    var personaje = await _db.Personajes
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);

    return personaje == null ? null : ConvertirDto(personaje);
}

    public async Task<PersonajeDto> CrearAsync(PersonajeCrearDto dto)
{
    var personaje = new Personaje
    {
        Nombre = dto.Nombre,
        Especie = dto.Especie,
        Faccion = dto.Faccion,
        Afiliacion = dto.Afiliacion,
        Estado = dto.Estado,
        FuerzaSensitivo = dto.FuerzaSensitivo,
        UbicacionSecretaCifrada =
            _proteccionService.Cifrar(dto.UbicacionSecreta)
    };

    _db.Personajes.Add(personaje);

    await _db.SaveChangesAsync();

    return ConvertirDto(personaje);
}

    
public async Task<ResultadoActualizarPersonaje> ActualizarAsync(
    int id,
    PersonajeCrearDto dto)
{
    var personaje = await _db.Personajes
        .FirstOrDefaultAsync(p => p.Id == id);

    if (personaje is null)
    {
        return ResultadoActualizarPersonaje.NoEncontrado;
    }

    bool tieneMuerteRegistrada = await _db.ParticipacionesEventos
        .AnyAsync(p => p.PersonajeId == id && p.MurioEnEvento);

    if (tieneMuerteRegistrada &&
        dto.Estado != EstadoPersonaje.Muerto)
    {
        return ResultadoActualizarPersonaje.MuerteRegistrada;
    }
    if (!dto.FuerzaSensitivo)
{
    bool tieneArmaRestringida = await _db.Armas
        .AnyAsync(a =>
            a.RequiereFuerzaSensitivo &&
            a.Personajes.Any(p => p.Id == id));

    if (tieneArmaRestringida)
    {
        return ResultadoActualizarPersonaje.ArmaRequiereFuerza;
    }
}
    personaje.Nombre = dto.Nombre;
    personaje.Especie = dto.Especie;
    personaje.Faccion = dto.Faccion;
    personaje.Afiliacion = dto.Afiliacion;
    personaje.Estado = dto.Estado;
    personaje.FuerzaSensitivo = dto.FuerzaSensitivo;

    if (!string.IsNullOrWhiteSpace(dto.UbicacionSecreta))
    {
        personaje.UbicacionSecretaCifrada =
            _proteccionService.Cifrar(dto.UbicacionSecreta);
    }

    await _db.SaveChangesAsync();

    return ResultadoActualizarPersonaje.Actualizado;
}


    public async Task<ResultadoEliminarPersonaje> EliminarAsync(int id)
    {
        var personaje = await _db.Personajes
            .FirstOrDefaultAsync(p => p.Id == id);

        if (personaje is null)
        {
            return ResultadoEliminarPersonaje.NoEncontrado;
        }

        bool tieneCarta = await _db.Cartas
            .AnyAsync(c => c.PersonajeId == id);

        if (tieneCarta)
        {
            return ResultadoEliminarPersonaje.TieneCarta;
        }

        bool tieneEventos = await _db.ParticipacionesEventos
            .AnyAsync(p => p.PersonajeId == id);

        if (tieneEventos)
        {
            return ResultadoEliminarPersonaje.TieneEventos;
        }
        bool tieneArmas = await _db.Armas
            .AnyAsync(a => a.Personajes.Any(p => p.Id == id));

        if (tieneArmas)
        {
            return ResultadoEliminarPersonaje.TieneArmas;
        }
        _db.Personajes.Remove(personaje);

        await _db.SaveChangesAsync();

        return ResultadoEliminarPersonaje.Eliminado;
    }

    public async Task<List<EventoDto>?> ObtenerEventosAsync(int personajeId)
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

        return eventos.Select(e => new EventoDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Fecha = e.Fecha,
            Ubicacion = e.Ubicacion,
            Descripcion = e.Descripcion,
            GanadorId = e.GanadorId,
            Resultado = e.Resultado,

            ParticipantesIds = e.Participaciones
                .Select(p => p.PersonajeId)
                .ToList(),

            MuertosIds = e.Participaciones
                .Where(p => p.MurioEnEvento)
                .Select(p => p.PersonajeId)
                .ToList()
        }).ToList();
    }

    
public async Task<List<object>> ObtenerRankingPorPoderAsync()
{
    var ranking = await (
        from personaje in _db.Personajes.AsNoTracking()
        join carta in _db.Cartas.AsNoTracking()
            on personaje.Id equals carta.PersonajeId

        let bonificacion = personaje.Armas.Sum(
            a => a.BonificacionPoder)

        orderby carta.Poder + bonificacion descending

        select new
        {
            PersonajeId = personaje.Id,
            Nombre = personaje.Nombre,
            PoderBase = carta.Poder,
            BonificacionArmas = bonificacion,
            Poder = carta.Poder + bonificacion
        }
    ).ToListAsync();

    return ranking.Cast<object>().ToList();
}

public async Task<(bool ExistePersonaje, object? Poder)>
    ObtenerPoderAsync(int id)
{
    var personaje = await _db.Personajes
        .AsNoTracking()
        .Include(p => p.CardPersonaje)
        .Include(p => p.Armas)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (personaje == null)
    {
        return (false, null);
    }

    if (personaje.CardPersonaje == null)
    {
        return (true, null);
    }

    int poderBase = personaje.CardPersonaje.Poder;

    int bonificacion = personaje.Armas.Sum(
        a => a.BonificacionPoder);

    int poderEfectivo = poderBase + bonificacion;

    return (true, new
    {
        PersonajeId = personaje.Id,
        Nombre = personaje.Nombre,
        PoderBase = poderBase,
        BonificacionArmas = bonificacion,
        PoderEfectivo = poderEfectivo
    });
}

    private PersonajeDto ConvertirDto(Personaje personaje)
{
    return new PersonajeDto
    {
        Id = personaje.Id,
        Nombre = personaje.Nombre,
        Especie = personaje.Especie,
        Faccion = personaje.Faccion,
        Afiliacion = personaje.Afiliacion,
        Estado = personaje.Estado,
        FuerzaSensitivo = personaje.FuerzaSensitivo,
        UbicacionSecreta = _proteccionService.Descifrar(
            personaje.UbicacionSecretaCifrada
        )
    };
}
}
