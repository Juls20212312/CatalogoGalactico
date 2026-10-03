using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public class CardService
{
    private readonly DataStore _data;

    public CardService(DataStore data)
    {
        _data = data;
    }

    public IEnumerable<CardPersonaje> ObtenerTodos()
    {
        return _data.Cartas;
    }

    public CardPersonaje? ObtenerPorId(int id)
    {
        return _data.Cartas.FirstOrDefault(c => c.Id == id);
    }

    public CardPersonaje? ObtenerPorPersonajeId(int personajeId)
    {
        return _data.Cartas.FirstOrDefault(
            c => c.PersonajeId == personajeId);
    }

    public bool ExistePersonaje(int personajeId)
    {
        return _data.Personajes.Any(p => p.Id == personajeId);
    }

    public bool YaTieneCarta(int personajeId, int? cartaIdExcluir = null)
    {
        return _data.Cartas.Any(c =>
            c.PersonajeId == personajeId &&
            c.Id != cartaIdExcluir);
    }

    public CardPersonaje Crear(CardRequest request)
    {
        int nuevoId = _data.Cartas.Count == 0
            ? 1
            : _data.Cartas.Max(c => c.Id) + 1;

        var carta = new CardPersonaje
        {
            Id = nuevoId,
            PersonajeId = request.PersonajeId,
            Poder = request.Poder,
            HabilidadEspecial = request.HabilidadEspecial,
            Arma = request.Arma,
            NivelPeligrosidad = request.NivelPeligrosidad,
            ImagenUrl = request.ImagenUrl
        };

        _data.Cartas.Add(carta);

        return carta;
    }

    public bool Actualizar(int id, CardRequest request)
    {
        var carta = ObtenerPorId(id);

        if (carta is null)
        {
            return false;
        }

        carta.PersonajeId = request.PersonajeId;
        carta.Poder = request.Poder;
        carta.HabilidadEspecial = request.HabilidadEspecial;
        carta.Arma = request.Arma;
        carta.NivelPeligrosidad = request.NivelPeligrosidad;
        carta.ImagenUrl = request.ImagenUrl;

        return true;
    }

    public bool Eliminar(int id)
    {
        var carta = ObtenerPorId(id);

        if (carta is null)
        {
            return false;
        }

        _data.Cartas.Remove(carta);

        return true;
    }
}