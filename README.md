# Catálogo Galáctico

## Descripción

Catálogo Galáctico es una API REST desarrollada con ASP.NET Core Web API en .NET 10. Permite administrar personajes, cartas coleccionables y eventos de un universo inspirado en Star Wars.

El proyecto comenzó como una Minimal API con datos almacenados en memoria. Posteriormente se convirtió en una Web API con controladores, utilizando Entity Framework Core y MySQL para almacenar los datos de forma permanente.

También incorpora un sistema de armas con reglas de compatibilidad y bonificaciones de poder.

## Tecnologías utilizadas

- C# y .NET 10
- ASP.NET Core Web API con controladores
- Entity Framework Core
- MySQL y Pomelo.EntityFrameworkCore.MySql
- Swagger / OpenAPI
- Secret Manager
- PasswordHasher para contraseñas
- AES-GCM para cifrado de datos sensibles

## Funcionalidades

El sistema permite crear, consultar, actualizar y eliminar personajes, cartas y eventos.

Los personajes tienen una carta que determina su poder base. También pueden recibir armas que aumentan su poder efectivo, respetando las restricciones de compatibilidad con la Fuerza.

El sistema incluye filtros de personajes, ranking por poder, MVP de eventos y simulación de batallas.

Además, controla las muertes registradas en eventos para impedir participaciones posteriores.

## Base de datos

El proyecto utiliza MySQL con Entity Framework Core.

Las entidades principales son Personaje, CardPersonaje, Evento, ParticipacionEvento, Arma y Usuario.

Las relaciones incluyen una carta principal por personaje, múltiples participaciones de personajes en eventos y asignaciones de armas mediante una tabla intermedia.

## Requisitos

Se necesita tener instalado .NET SDK 10, MySQL Server y una herramienta para ejecutar consultas SQL, como MySQL Workbench.

También es necesario disponer de Entity Framework Core CLI (`dotnet ef`) para aplicar las migraciones.

## Configuración de secretos

Por seguridad, la contraseña de MySQL y la clave de cifrado no se guardan directamente en el código ni en `appsettings.json`.

Se utiliza Secret Manager.

Ejemplo de configuración de conexión con datos ficticios:

`dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=catalogo_galactico;User=usuario_ejemplo;Password=CLAVE_FICTICIA"`

Para la clave de cifrado debe utilizarse una clave aleatoria de 32 bytes codificada en Base64, almacenada con el identificador `Encryption:Key`.

La clave utilizada para cifrar los datos debe conservarse de forma segura.

## Migraciones

El proyecto contiene las siguientes migraciones:

| Migración | Función |
|---|---|
| Inicial | Creación de las entidades y relaciones iniciales |
| SistemaArmas | Incorporación del catálogo y asignaciones de armas |
| ProteccionDatos | Incorporación del campo cifrado del personaje |

Para aplicar las migraciones, después de configurar los secretos, ejecutar:

`dotnet ef database update --context AppDbContext`

## Ejecutar el proyecto

Desde la carpeta del proyecto, ejecutar:

`dotnet restore`

`dotnet build`

`dotnet run`

En el entorno de desarrollo se puede utilizar Swagger en la dirección local indicada por la aplicación, agregando `/swagger`.

## Protección de datos

Las contraseñas de los usuarios se almacenan como hashes generados con PasswordHasher. La API no devuelve el hash en sus respuestas.

La ubicación secreta de los personajes se cifra con AES-GCM antes de almacenarse en MySQL. Al consultar un personaje, se descifra para construir el DTO de respuesta.

La cadena de conexión y la clave de cifrado se administran mediante Secret Manager.

## Componente propio: sistema de armas

El componente adicional permite registrar armas, asignarlas a personajes y calcular su bonificación de poder.

Algunas armas requieren sensibilidad a la Fuerza. El sistema impide asignarlas a personajes incompatibles y evita las asignaciones duplicadas.

El poder efectivo se calcula sumando el poder base de la carta y las bonificaciones de las armas asignadas. Este valor se utiliza en el ranking, MVP y simulación de batallas.