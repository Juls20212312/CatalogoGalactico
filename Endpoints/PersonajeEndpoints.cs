using CatalogoGalactico.Models;
using CatalogoGalactico.Services;

namespace CatalogoGalactico.Endpoints;

public static class PersonajeEndpoints
{
    public static void MapPersonajeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/personajes")
            .WithTags("Personajes");

        group.MapGet("/", (
            PersonajeService service,
            Faccion? faccion,
            bool? fuerzaSensitivo) =>
        {
            var personajes = service.ObtenerTodos(
                faccion,
                fuerzaSensitivo);

            return Results.Ok(personajes);
        });

        group.MapGet("/ranking", (string? por,PersonajeService service) =>
{
    if (!string.IsNullOrWhiteSpace(por) &&
        !por.Equals("poder", StringComparison.OrdinalIgnoreCase))
    {
        return Results.BadRequest(new
        {
            mensaje = "El parámetro 'por' debe ser 'poder'."
        });
    }

    return Results.Ok(service.ObtenerRankingPorPoder());
});
        group.MapGet("/{id:int}/con-card", (
    int id,
    PersonajeService service) =>
{
    var ficha = service.ObtenerConCard(id);

    return ficha is null
        ? Results.NotFound(new
        {
            mensaje = "Personaje no encontrado."
        })
        : Results.Ok(ficha);
});
        group.MapGet("/{id:int}", (
            int id,
            PersonajeService service) =>
        {
            var personaje = service.ObtenerPorId(id);

            return personaje is null
                ? Results.NotFound(new
                {
                    mensaje = "Personaje no encontrado."
                })
                : Results.Ok(personaje);
        });

        group.MapGet("/{id:int}/eventos", (
            int id,
            PersonajeService service) =>
        {
            var personaje = service.ObtenerPorId(id);

            if (personaje is null)
            {
                return Results.NotFound(new
                {
                    mensaje = "Personaje no encontrado."
                });
            }

            return Results.Ok(service.ObtenerEventos(id));
        });

        group.MapPost("/", (
            PersonajeRequest request,
            PersonajeService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return Results.BadRequest(new
                {
                    mensaje = "El nombre es obligatorio."
                });
            }

            var personaje = service.Crear(request);

            return Results.Created(
                $"/personajes/{personaje.Id}",
                personaje);
        });

        group.MapPut("/{id:int}", (
    int id,
    PersonajeRequest request,
    PersonajeService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Nombre))
    {
        return Results.BadRequest(new
        {
            mensaje = "El nombre es obligatorio."
        });
    }

    var resultado = service.Actualizar(id, request);

    if (resultado.Error == "NOT_FOUND")
    {
        return Results.NotFound(new
        {
            mensaje = "Personaje no encontrado."
        });
    }

    if (resultado.Error is not null)
    {
        return Results.BadRequest(new
        {
            mensaje = resultado.Error
        });
    }

    return Results.Ok(resultado.Personaje);
});

        group.MapDelete("/{id:int}", (
    int id,
    PersonajeService service) =>
{
    var resultado = service.Eliminar(id);

    if (resultado == "NOT_FOUND")
    {
        return Results.NotFound(new
        {
            mensaje = "Personaje no encontrado."
        });
    }

    if (resultado is not null)
    {
        return Results.BadRequest(new
        {
            mensaje = resultado
        });
    }

    return Results.NoContent();
});
    }
}