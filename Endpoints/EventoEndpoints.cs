using CatalogoGalactico.Models;
using CatalogoGalactico.Services;

namespace CatalogoGalactico.Endpoints;

public static class EventoEndpoints
{
    public static void MapEventoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/eventos")
            .WithTags("Eventos");

        group.MapGet("/", (EventoService service) =>
        {
            return Results.Ok(service.ObtenerTodos());
        });

        group.MapGet("/{id:int}", (
            int id,
            EventoService service) =>
        {
            var evento = service.ObtenerPorId(id);

            return evento is null
                ? Results.NotFound(new
                {
                    mensaje = "Evento no encontrado."
                })
                : Results.Ok(evento);
        });

        group.MapPost("/", (
            EventoRequest request,
            EventoService service) =>
        {
            var error = service.ValidarEvento(request);

            if (error is not null)
            {
                return Results.BadRequest(new
                {
                    mensaje = error
                });
            }

            var evento = service.Crear(request);

            return Results.Created(
                $"/eventos/{evento.Id}",
                evento);
        });

        group.MapPut("/{id:int}", (
            int id,
            EventoRequest request,
            EventoService service) =>
        {
            if (service.ObtenerPorId(id) is null)
            {
                return Results.NotFound(new
                {
                    mensaje = "Evento no encontrado."
                });
            }

            var error = service.ValidarEvento(
                request,
                id);

            if (error is not null)
            {
                return Results.BadRequest(new
                {
                    mensaje = error
                });
            }

            service.Actualizar(id, request);

            return Results.Ok(
                service.ObtenerPorId(id));
        });

        group.MapDelete("/{id:int}", (
            int id,
            EventoService service) =>
        {
            var eliminado = service.Eliminar(id);

            return eliminado
                ? Results.NoContent()
                : Results.NotFound(new
                {
                    mensaje = "Evento no encontrado."
                });
        });

        group.MapGet("/{id:int}/mvp", (
            int id,
            EventoService service) =>
        {
            var evento = service.ObtenerPorId(id);

            if (evento is null)
            {
                return Results.NotFound(new
                {
                    mensaje = "Evento no encontrado."
                });
            }

            var mvp = service.ObtenerMvp(id);

            if (mvp is null)
            {
                return Results.BadRequest(new
                {
                    mensaje = "No se puede calcular el MVP porque los participantes no tienen cartas."
                });
            }

            return Results.Ok(mvp);
        });

        group.MapPost("/{id:int}/simular", (
            int id,
            EventoService service) =>
        {
            var evento = service.ObtenerPorId(id);

            if (evento is null)
            {
                return Results.NotFound(new
                {
                    mensaje = "Evento no encontrado."
                });
            }

            try
            {
                var resultado = service.Simular(id);

                return Results.Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        });
    }
}