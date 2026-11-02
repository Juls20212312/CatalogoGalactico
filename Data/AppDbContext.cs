
using CatalogoGalactico.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Personaje> Personajes => Set<Personaje>();

    public DbSet<CardPersonaje> Cartas => Set<CardPersonaje>();

    public DbSet<Evento> Eventos => Set<Evento>();

    public DbSet<ParticipacionEvento> ParticipacionesEventos => Set<ParticipacionEvento>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Arma> Armas => Set<Arma>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Personaje>(e =>
        {
            e.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(p => p.Especie)
                .IsRequired()
                .HasMaxLength(80);

            e.Property(p => p.Faccion)
                .HasConversion<string>()
                .HasMaxLength(20);

            e.Property(p => p.Afiliacion)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(p => p.Estado)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        modelBuilder.Entity<CardPersonaje>(e =>
        {
            e.Property(c => c.HabilidadEspecial)
                .IsRequired()
                .HasMaxLength(200);

            e.Property(c => c.Arma)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(c => c.ImagenUrl)
                .HasMaxLength(500);

            e.HasOne(c => c.Personaje)
                .WithOne(p => p.CardPersonaje)
                .HasForeignKey<CardPersonaje>(c => c.PersonajeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Evento>(e =>
        {
            e.Property(ev => ev.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            e.Property(ev => ev.Ubicacion)
                .IsRequired()
                .HasMaxLength(150);

            e.Property(ev => ev.Descripcion)
                .HasMaxLength(500);

            e.Property(ev => ev.Resultado)
                .HasMaxLength(500);

            e.Ignore(ev => ev.ParticipantesIds);

            e.Ignore(ev => ev.MuertosIds);

            e.HasOne<Personaje>()
                .WithMany()
                .HasForeignKey(ev => ev.GanadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ParticipacionEvento>(e =>
        {
            e.HasKey(p => new { p.EventoId, p.PersonajeId });

            e.HasOne(p => p.Evento)
                .WithMany(ev => ev.Participaciones)
                .HasForeignKey(p => p.EventoId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Personaje)
                .WithMany(pj => pj.Participaciones)
                .HasForeignKey(p => p.PersonajeId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => p.PersonajeId);
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            e.HasIndex(u => u.Email)
                .IsUnique();

            e.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
        
        });
        modelBuilder.Entity<Personaje>()
        .HasMany(p => p.Armas)
        .WithMany(a => a.Personajes)
        .UsingEntity<Dictionary<string, object>>(
        "PersonajeArma",
        derecha => derecha
            .HasOne<Arma>()
            .WithMany()
            .HasForeignKey("ArmaId")
            .OnDelete(DeleteBehavior.Cascade),
        izquierda => izquierda
            .HasOne<Personaje>()
            .WithMany()
            .HasForeignKey("PersonajeId")
            .OnDelete(DeleteBehavior.Cascade),
        relacion =>
        {
            relacion.ToTable("PersonajeArma");
            relacion.HasKey("PersonajeId", "ArmaId");
        });

        // PERSONAJES INICIALES

        modelBuilder.Entity<Personaje>().HasData(
            new Personaje
            {
                Id = 1,
                Nombre = "Luke Skywalker",
                Especie = "Humano",
                Faccion = Faccion.Rebelde,
                Afiliacion = "Alianza Rebelde",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = true
            },
            new Personaje
            {
                Id = 2,
                Nombre = "Leia Organa",
                Especie = "Humana",
                Faccion = Faccion.Rebelde,
                Afiliacion = "Alianza Rebelde",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = true
            },
            new Personaje
            {
                Id = 3,
                Nombre = "Han Solo",
                Especie = "Humano",
                Faccion = Faccion.Rebelde,
                Afiliacion = "Alianza Rebelde",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = false
            },
            new Personaje
            {
                Id = 4,
                Nombre = "Darth Vader",
                Especie = "Humano",
                Faccion = Faccion.Imperio,
                Afiliacion = "Imperio Galáctico",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = true
            },
            new Personaje
            {
                Id = 5,
                Nombre = "Emperador Palpatine",
                Especie = "Humano",
                Faccion = Faccion.Imperio,
                Afiliacion = "Imperio Galáctico",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = true
            },
            new Personaje
            {
                Id = 6,
                Nombre = "Boba Fett",
                Especie = "Humano",
                Faccion = Faccion.Neutral,
                Afiliacion = "Cazarrecompensas",
                Estado = EstadoPersonaje.Vivo,
                FuerzaSensitivo = false
            }
        );

        // CARTAS INICIALES

        modelBuilder.Entity<CardPersonaje>().HasData(
            new CardPersonaje
            {
                Id = 1,
                PersonajeId = 1,
                Poder = 100,
                HabilidadEspecial = "Dominio de la Fuerza",
                Arma = "Sable de luz",
                NivelPeligrosidad = 8,
                ImagenUrl = "https://example.com/luke.jpg"
            },
            new CardPersonaje
            {
                Id = 2,
                PersonajeId = 2,
                Poder = 85,
                HabilidadEspecial = "Liderazgo Rebelde",
                Arma = "Bláster",
                NivelPeligrosidad = 7,
                ImagenUrl = "https://example.com/leia.jpg"
            },
            new CardPersonaje
            {
                Id = 3,
                PersonajeId = 3,
                Poder = 82,
                HabilidadEspecial = "Piloto experto",
                Arma = "Bláster pesado",
                NivelPeligrosidad = 7,
                ImagenUrl = "https://example.com/han.jpg"
            },
            new CardPersonaje
            {
                Id = 4,
                PersonajeId = 4,
                Poder = 100,
                HabilidadEspecial = "Poder del lado oscuro",
                Arma = "Sable de luz rojo",
                NivelPeligrosidad = 10,
                ImagenUrl = "https://example.com/vader.jpg"
            },
            new CardPersonaje
            {
                Id = 5,
                PersonajeId = 5,
                Poder = 110,
                HabilidadEspecial = "Rayos de la Fuerza",
                Arma = "Bastón ceremonial",
                NivelPeligrosidad = 10,
                ImagenUrl = "https://example.com/palpatine.jpg"
            },
            new CardPersonaje
            {
                Id = 6,
                PersonajeId = 6,
                Poder = 75,
                HabilidadEspecial = "Caza de objetivos",
                Arma = "Bláster",
                NivelPeligrosidad = 8,
                ImagenUrl = "https://example.com/boba.jpg"
            }
        );

        // EVENTOS INICIALES

        modelBuilder.Entity<Evento>().HasData(
            new Evento
            {
                Id = 1,
                Nombre = "Batalla de Yavin",
                Fecha = 0,
                Ubicacion = "Estrella de la Muerte",
                Descripcion = "Enfrentamiento decisivo de la Alianza Rebelde contra el Imperio.",
                GanadorId = 1,
                Resultado = "Victoria de la Alianza Rebelde"
            },
            new Evento
            {
                Id = 2,
                Nombre = "Batalla de Endor",
                Fecha = 4,
                Ubicacion = "Endor",
                Descripcion = "Batalla terrestre y espacial contra el Imperio Galáctico.",
                GanadorId = 1,
                Resultado = "Victoria de la Alianza Rebelde"
            }
        );

        // PARTICIPANTES INICIALES DE LOS EVENTOS

        modelBuilder.Entity<ParticipacionEvento>().HasData(
            new ParticipacionEvento { EventoId = 1, PersonajeId = 1, MurioEnEvento = false },
            new ParticipacionEvento { EventoId = 1, PersonajeId = 2, MurioEnEvento = false },
            new ParticipacionEvento { EventoId = 1, PersonajeId = 3, MurioEnEvento = false },
            new ParticipacionEvento { EventoId = 1, PersonajeId = 4, MurioEnEvento = false },

            new ParticipacionEvento { EventoId = 2, PersonajeId = 1, MurioEnEvento = false },
            new ParticipacionEvento { EventoId = 2, PersonajeId = 2, MurioEnEvento = false },
            new ParticipacionEvento { EventoId = 2, PersonajeId = 3, MurioEnEvento = false },
            new ParticipacionEvento { EventoId = 2, PersonajeId = 4, MurioEnEvento = false },
            new ParticipacionEvento { EventoId = 2, PersonajeId = 5, MurioEnEvento = false }
        );
    }
}
