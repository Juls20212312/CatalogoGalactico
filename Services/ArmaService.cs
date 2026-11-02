using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class ArmaService : IArmaService
{
    private readonly AppDbContext _db;

    public ArmaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ArmaDto>> ObtenerTodasAsync()
    {
        var armas = await _db.Armas
            .AsNoTracking()
            .Include(a => a.Personajes)
            .OrderBy(a => a.Nombre)
            .ToListAsync();

        return armas.Select(ConvertirDto).ToList();
    }

    public async Task<ArmaDto?> ObtenerPorIdAsync(int id)
    {
        var arma = await _db.Armas
            .AsNoTracking()
            .Include(a => a.Personajes)
            .FirstOrDefaultAsync(a => a.Id == id);

        return arma == null ? null : ConvertirDto(arma);
    }

    public async Task<ArmaDto> CrearAsync(ArmaCrearDto dto)
    {
        var arma = new Arma
        {
            Nombre = dto.Nombre.Trim(),
            Tipo = dto.Tipo.Trim(),
            BonificacionPoder = dto.BonificacionPoder,
            RequiereFuerzaSensitivo = dto.RequiereFuerzaSensitivo,
            Descripcion = dto.Descripcion.Trim()
        };

        _db.Armas.Add(arma);

        await _db.SaveChangesAsync();

        return ConvertirDto(arma);
    }

    public async Task<ResultadoCambioArma> ActualizarAsync(
    int id,
    ArmaCrearDto dto)
{
    var arma = await _db.Armas
        .Include(a => a.Personajes)
        .FirstOrDefaultAsync(a => a.Id == id);

    if (arma == null)
    {
        return ResultadoCambioArma.NoEncontrada;
    }

    if (dto.RequiereFuerzaSensitivo &&
        arma.Personajes.Any(p => !p.FuerzaSensitivo))
    {
        return ResultadoCambioArma.Conflicto;
    }

    arma.Nombre = dto.Nombre.Trim();
    arma.Tipo = dto.Tipo.Trim();
    arma.BonificacionPoder = dto.BonificacionPoder;
    arma.RequiereFuerzaSensitivo = dto.RequiereFuerzaSensitivo;
    arma.Descripcion = dto.Descripcion.Trim();

    await _db.SaveChangesAsync();

    return ResultadoCambioArma.Correcto;
}

    
public async Task<ResultadoCambioArma> EliminarAsync(int id)
{
    var arma = await _db.Armas
        .Include(a => a.Personajes)
        .FirstOrDefaultAsync(a => a.Id == id);

    if (arma == null)
    {
        return ResultadoCambioArma.NoEncontrada;
    }

    if (arma.Personajes.Any())
    {
        return ResultadoCambioArma.Conflicto;
    }

    _db.Armas.Remove(arma);

    await _db.SaveChangesAsync();

    return ResultadoCambioArma.Correcto;
}


    public async Task<ResultadoAsignacionArma> AsignarAsync(
        int armaId,
        int personajeId)
    {
        var arma = await _db.Armas
            .Include(a => a.Personajes)
            .FirstOrDefaultAsync(a => a.Id == armaId);

        if (arma == null)
        {
            return ResultadoAsignacionArma.ArmaNoExiste;
        }

        var personaje = await _db.Personajes
            .FirstOrDefaultAsync(p => p.Id == personajeId);

        if (personaje == null)
        {
            return ResultadoAsignacionArma.PersonajeNoExiste;
        }

        if (arma.Personajes.Any(p => p.Id == personajeId))
        {
            return ResultadoAsignacionArma.YaAsignada;
        }

        if (arma.RequiereFuerzaSensitivo &&
            !personaje.FuerzaSensitivo)
        {
            return ResultadoAsignacionArma.RequiereFuerza;
        }

        arma.Personajes.Add(personaje);

        await _db.SaveChangesAsync();

        return ResultadoAsignacionArma.Asignada;
    }

    public async Task<ResultadoDesasignacionArma> DesasignarAsync(
        int armaId,
        int personajeId)
    {
        var arma = await _db.Armas
            .Include(a => a.Personajes)
            .FirstOrDefaultAsync(a => a.Id == armaId);

        if (arma == null)
        {
            return ResultadoDesasignacionArma.ArmaNoExiste;
        }

        var personaje = await _db.Personajes
            .FirstOrDefaultAsync(p => p.Id == personajeId);

        if (personaje == null)
        {
            return ResultadoDesasignacionArma.PersonajeNoExiste;
        }

        var asignado = arma.Personajes
            .FirstOrDefault(p => p.Id == personajeId);

        if (asignado == null)
        {
            return ResultadoDesasignacionArma.NoEstabaAsignada;
        }

        arma.Personajes.Remove(asignado);

        await _db.SaveChangesAsync();

        return ResultadoDesasignacionArma.Desasignada;
    }

    private static ArmaDto ConvertirDto(Arma arma)
    {
        return new ArmaDto
        {
            Id = arma.Id,
            Nombre = arma.Nombre,
            Tipo = arma.Tipo,
            BonificacionPoder = arma.BonificacionPoder,
            RequiereFuerzaSensitivo = arma.RequiereFuerzaSensitivo,
            Descripcion = arma.Descripcion,
            PersonajesIds = arma.Personajes
                .Select(p => p.Id)
                .ToList()
        };
    }
}