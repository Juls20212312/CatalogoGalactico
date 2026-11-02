namespace CatalogoGalactico.Models;

public class ParticipacionEvento
{
    public int EventoId { get; set; }

    public Evento Evento { get; set; } = null!;

    public int PersonajeId { get; set; }

    public Personaje Personaje { get; set; } = null!;

    public bool MurioEnEvento { get; set; }
}   