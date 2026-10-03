# Catálogo Galáctico de Personajes y Eventos

Proyecto académico para la materia Tecnologías Web 1.

## Descripción

La aplicación implementa una API REST utilizando ASP.NET Core Minimal API.

El sistema permite administrar:

- Personajes
- Cartas coleccionables
- Eventos

Los datos se almacenan en memoria mediante tres listas, por lo que los cambios se pierden al reiniciar la aplicación.

## Tecnologías utilizadas

- C#
- ASP.NET Core Minimal API
- .NET 10
- Swagger / OpenAPI
- Almacenamiento en memoria
- REST API

## Estructura

```text
CatalogoGalactico/
│
├── Models/
│   ├── Personaje.cs
│   ├── CardPersonaje.cs
│   ├── Evento.cs
│   ├── Enums.cs
│   └── Dtos.cs
│
├── Data/
│   └── DataStore.cs
│
├── Services/
│   ├── PersonajeService.cs
│   ├── CardService.cs
│   └── EventoService.cs
│
├── Endpoints/
│   ├── PersonajeEndpoints.cs
│   ├── CardEndpoints.cs
│   └── EventoEndpoints.cs
│
├── Program.cs
└── CatalogoGalactico.csproj