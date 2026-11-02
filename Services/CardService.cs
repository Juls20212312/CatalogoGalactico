using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class CardService : ICardService
{
    private readonly AppDbContext _db;

    public CardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CardPersonajeDto>> ObtenerTodosAsync()
    {
        return await _db.Cartas
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CardPersonajeDto
            {
                Id = c.Id,
                PersonajeId = c.PersonajeId,
                Poder = c.Poder,
                HabilidadEspecial = c.HabilidadEspecial,
                Arma = c.Arma,
                NivelPeligrosidad = c.NivelPeligrosidad,
                ImagenUrl = c.ImagenUrl
            })
            .ToListAsync();
    }

    public async Task<CardPersonajeDto?> ObtenerPorIdAsync(int id)
    {
        return await _db.Cartas
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CardPersonajeDto
            {
                Id = c.Id,
                PersonajeId = c.PersonajeId,
                Poder = c.Poder,
                HabilidadEspecial = c.HabilidadEspecial,
                Arma = c.Arma,
                NivelPeligrosidad = c.NivelPeligrosidad,
                ImagenUrl = c.ImagenUrl
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CardPersonajeDto?> ObtenerPorPersonajeIdAsync(
        int personajeId)
    {
        return await _db.Cartas
            .AsNoTracking()
            .Where(c => c.PersonajeId == personajeId)
            .Select(c => new CardPersonajeDto
            {
                Id = c.Id,
                PersonajeId = c.PersonajeId,
                Poder = c.Poder,
                HabilidadEspecial = c.HabilidadEspecial,
                Arma = c.Arma,
                NivelPeligrosidad = c.NivelPeligrosidad,
                ImagenUrl = c.ImagenUrl
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(ResultadoOperacionCarta Resultado,
        CardPersonajeDto? Carta)> CrearAsync(CardPersonajeCrearDto dto)
    {
        if (dto.Poder < 0)
        {
            return (ResultadoOperacionCarta.PoderInvalido, null);
        }

        bool existePersonaje = await _db.Personajes
            .AnyAsync(p => p.Id == dto.PersonajeId);

        if (!existePersonaje)
        {
            return (ResultadoOperacionCarta.PersonajeNoExiste, null);
        }

        bool yaTieneCarta = await _db.Cartas
            .AnyAsync(c => c.PersonajeId == dto.PersonajeId);

        if (yaTieneCarta)
        {
            return (ResultadoOperacionCarta.PersonajeYaTieneCarta, null);
        }

        var carta = new CardPersonaje
        {
            PersonajeId = dto.PersonajeId,
            Poder = dto.Poder,
            HabilidadEspecial = dto.HabilidadEspecial,
            Arma = dto.Arma,
            NivelPeligrosidad = dto.NivelPeligrosidad,
            ImagenUrl = dto.ImagenUrl
        };

        _db.Cartas.Add(carta);

        await _db.SaveChangesAsync();

        var respuesta = new CardPersonajeDto
        {
            Id = carta.Id,
            PersonajeId = carta.PersonajeId,
            Poder = carta.Poder,
            HabilidadEspecial = carta.HabilidadEspecial,
            Arma = carta.Arma,
            NivelPeligrosidad = carta.NivelPeligrosidad,
            ImagenUrl = carta.ImagenUrl
        };

        return (ResultadoOperacionCarta.Correcto, respuesta);
    }

    public async Task<ResultadoOperacionCarta> ActualizarAsync(
        int id,
        CardPersonajeCrearDto dto)
    {
        var carta = await _db.Cartas
            .FirstOrDefaultAsync(c => c.Id == id);

        if (carta == null)
        {
            return ResultadoOperacionCarta.NoEncontrada;
        }

        if (dto.Poder < 0)
        {
            return ResultadoOperacionCarta.PoderInvalido;
        }

        bool existePersonaje = await _db.Personajes
            .AnyAsync(p => p.Id == dto.PersonajeId);

        if (!existePersonaje)
        {
            return ResultadoOperacionCarta.PersonajeNoExiste;
        }

        bool otraCartaExiste = await _db.Cartas
            .AnyAsync(c => c.PersonajeId == dto.PersonajeId && c.Id != id);

        if (otraCartaExiste)
        {
            return ResultadoOperacionCarta.PersonajeYaTieneCarta;
        }

        carta.PersonajeId = dto.PersonajeId;
        carta.Poder = dto.Poder;
        carta.HabilidadEspecial = dto.HabilidadEspecial;
        carta.Arma = dto.Arma;
        carta.NivelPeligrosidad = dto.NivelPeligrosidad;
        carta.ImagenUrl = dto.ImagenUrl;

        await _db.SaveChangesAsync();

        return ResultadoOperacionCarta.Correcto;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var carta = await _db.Cartas
            .FirstOrDefaultAsync(c => c.Id == id);

        if (carta == null)
        {
            return false;
        }

        _db.Cartas.Remove(carta);

        await _db.SaveChangesAsync();

        return true;
    }
}