using CatalogoGalactico.Models;

namespace CatalogoGalactico.Data;

public class DataStore
{
    public List<Personaje> Personajes { get; } = new();

    public List<CardPersonaje> Cartas { get; } = new();

    public List<Evento> Eventos { get; } = new();

    public DataStore()
    {
        Personajes.AddRange(new[]
        {
            new Personaje
            {
                Id = 1,
                Nombre = "Luke Skywalker",
                Especie = "Humano",
                Faccion = Faccion.Rebelde,
                Afiliacion = "Alianza Rebelde",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = true,
                Image = "https://www.shutterstock.com/image-photo/portrait-luke-skywalker-light-saber-600w-2437278559.jpg"
            },

            new Personaje
            {
                Id = 2,
                Nombre = "Leia Organa",
                Especie = "Humana",
                Faccion = Faccion.Rebelde,
                Afiliacion = "Alianza Rebelde",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = true,
                Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR6K8uU6qycwKT2d2F3LtA7E3jUCalwgIwGBtofddkQUA&s=10"
            },

            new Personaje
            {
                Id = 3,
                Nombre = "Han Solo",
                Especie = "Humano",
                Faccion = Faccion.Rebelde,
                Afiliacion = "Alianza Rebelde",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = false,
                Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSu44_JQcN-i7hOreVazfWeglhUtZLvaXARgJ0P4_f7zg&s=10"
            },

            new Personaje
            {
                Id = 4,
                Nombre = "Darth Vader",
                Especie = "Humano",
                Faccion = Faccion.Imperio,
                Afiliacion = "Imperio Galáctico",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = true,
                Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcThnG0pBvATdmEfbFHi1ODwnynman5KIUslJ38NDlWBKQ&s=10"
            },

            new Personaje
            {
                Id = 5,
                Nombre = "Emperador Palpatine",
                Especie = "Humano",
                Faccion = Faccion.Imperio,
                Afiliacion = "Imperio Galáctico",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = true,
                Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRc8PygPceteniartp4JCrmZi-ipOgBmaVLuKncRK91-A&s"
            },

            new Personaje
            {
                Id = 6,
                Nombre = "Boba Fett",
                Especie = "Humano",
                Faccion = Faccion.Neutral,
                Afiliacion = "Cazarrecompensas",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = false,
                Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR3Zrwg4jd3LjdvdwxWylA75zcTokIIGHm_bF9mw9hoWQ&s=10"
            }
        });

        Cartas.AddRange(new[]
        {
            new CardPersonaje
            {
                Id = 1,
                PersonajeId = 1,
                Poder = 100,
                HabilidadEspecial = "Dominio de la Fuerza",
                Arma = "Sable de luz",
                NivelPeligrosidad = 8,
                ImagenUrl = "https://www.shutterstock.com/image-photo/portrait-luke-skywalker-light-saber-600w-2437278559.jpg"
            },

            new CardPersonaje
            {
                Id = 2,
                PersonajeId = 2,
                Poder = 85,
                HabilidadEspecial = "Liderazgo Rebelde",
                Arma = "Bláster",
                NivelPeligrosidad = 7,
                ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR6K8uU6qycwKT2d2F3LtA7E3jUCalwgIwGBtofddkQUA&s=10"
            },

            new CardPersonaje
            {
                Id = 3,
                PersonajeId = 3,
                Poder = 82,
                HabilidadEspecial = "Piloto experto",
                Arma = "Bláster pesado",
                NivelPeligrosidad = 7,
                ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSu44_JQcN-i7hOreVazfWeglhUtZLvaXARgJ0P4_f7zg&s=10"
            },

            new CardPersonaje
            {
                Id = 4,
                PersonajeId = 4,
                Poder = 100,
                HabilidadEspecial = "Poder del lado oscuro",
                Arma = "Sable de luz rojo",
                NivelPeligrosidad = 10,
                ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcThnG0pBvATdmEfbFHi1ODwnynman5KIUslJ38NDlWBKQ&s=10"
            },

            new CardPersonaje
            {
                Id = 5,
                PersonajeId = 5,
                Poder = 110,
                HabilidadEspecial = "Rayos de la Fuerza",
                Arma = "Bastón ceremonial",
                NivelPeligrosidad = 10,
                ImagenUrl = "https://www.shutterstock.com/image-photo/portrait-luke-skywalker-light-saber-600w-2437278559.jpg"
            },

            new CardPersonaje
            {
                Id = 6,
                PersonajeId = 6,
                Poder = 75,
                HabilidadEspecial = "Caza de objetivos",
                Arma = "Bláster",
                NivelPeligrosidad = 8,
                ImagenUrl = "https://www.shutterstock.com/image-photo/portrait-luke-skywalker-light-saber-600w-2437278559.jpg"
            }
        });

        Eventos.AddRange(new[]
        {
            new Evento
            {
                Id = 1,
                Nombre = "Batalla de Yavin",
                Fecha = 0,
                Ubicacion = "Estrella de la Muerte",
                Descripcion = "Enfrentamiento decisivo de la Alianza Rebelde contra el Imperio.",
                ParticipantesIds = new List<int> { 1, 2, 3, 4 },
                GanadorId = 1,
                Resultado = "Victoria de la Alianza Rebelde",
                MuertosIds = new List<int>()
            },

            new Evento
            {
                Id = 2,
                Nombre = "Batalla de Endor",
                Fecha = 4,
                Ubicacion = "Endor",
                Descripcion = "Batalla terrestre y espacial contra el Imperio Galáctico.",
                ParticipantesIds = new List<int> { 1, 2, 3, 4, 5 },
                GanadorId = 1,
                Resultado = "Victoria de la Alianza Rebelde",
                MuertosIds = new List<int>()
            }
        });
    }
}