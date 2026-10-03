using CatalogoGalactico.Models;
using CatalogoGalactico.Services;

namespace CatalogoGalactico.Endpoints;

public static class CardEndpoints
{
    public static void MapCardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/cartas")
            .WithTags("Cartas");

        group.MapGet("/", (CardService service) =>
        {
            return Results.Ok(service.ObtenerTodos());
        });

        group.MapGet("/{id:int}", (
            int id,
            CardService service) =>
        {
            var carta = service.ObtenerPorId(id);

            return carta is null
                ? Results.NotFound(new
                {
                    mensaje = "Carta no encontrada."
                })
                : Results.Ok(carta);
        });

        group.MapPost("/", (
            CardRequest request,
            CardService service) =>
        {
            if (!service.ExistePersonaje(request.PersonajeId))
            {
                return Results.BadRequest(new
                {
                    mensaje = "El personaje indicado no existe."
                });
            }

            if (service.YaTieneCarta(request.PersonajeId))
            {
                return Results.BadRequest(new
                {
                    mensaje = "El personaje ya tiene una carta."
                });
            }

            if (request.Poder < 0)
            {
                return Results.BadRequest(new
                {
                    mensaje = "El poder no puede ser negativo."
                });
            }

            var carta = service.Crear(request);

            return Results.Created(
                $"/cartas/{carta.Id}",
                carta);
        });

        group.MapPut("/{id:int}", (
            int id,
            CardRequest request,
            CardService service) =>
        {
            if (!service.ExistePersonaje(request.PersonajeId))
            {
                return Results.BadRequest(new
                {
                    mensaje = "El personaje indicado no existe."
                });
            }

            if (service.YaTieneCarta(
                    request.PersonajeId,
                    id))
            {
                return Results.BadRequest(new
                {
                    mensaje = "El personaje ya tiene otra carta."
                });
            }

            if (request.Poder < 0)
            {
                return Results.BadRequest(new
                {
                    mensaje = "El poder no puede ser negativo."
                });
            }

            var actualizado = service.Actualizar(
                id,
                request);

            return actualizado
                ? Results.Ok(service.ObtenerPorId(id))
                : Results.NotFound(new
                {
                    mensaje = "Carta no encontrada."
                });
        });

        group.MapDelete("/{id:int}", (
            int id,
            CardService service) =>
        {
            var eliminado = service.Eliminar(id);

            return eliminado
                ? Results.NoContent()
                : Results.NotFound(new
                {
                    mensaje = "Carta no encontrada."
                });
        });
    }
}