# Declaración de uso de inteligencia artificial

## 1. Herramientas utilizadas

Durante el desarrollo del proyecto Catálogo Galáctico utilicé **ChatGPT de OpenAI** como herramienta de apoyo para comprender requisitos, revisar código, identificar errores y recibir orientación sobre la implementación.

El proyecto se desarrolló utilizando C#, ASP.NET Core Web API, Entity Framework Core y MySQL.

## 2. Uso de la inteligencia artificial

Utilicé ChatGPT principalmente para convertir mi proyecto anterior, que era una Minimal API con almacenamiento en memoria, en una Web API con controladores y almacenamiento persistente en MySQL.

También lo utilicé para revisar las instrucciones del trabajo final, comparar la estructura con el proyecto de ejemplo del docente e implementar la protección de datos.

Trabajé realizando cambios por etapas, compilando el proyecto y probando las funcionalidades mediante Swagger y MySQL Workbench.

## 3. Consultas importantes realizadas

Entre las consultas que hice se encuentran:

- Cómo convertir mi Minimal API a una Web API con controladores sin perder las funcionalidades originales.
- Cómo configurar Entity Framework Core para utilizar MySQL.
- Cómo crear y aplicar migraciones sin perder los datos existentes.
- Cómo implementar el almacenamiento de contraseñas mediante hash.
- Cómo cifrar la ubicación secreta de los personajes.
- Cómo incorporar un sistema de armas con compatibilidad según la sensibilidad a la Fuerza.
- Cómo utilizar las bonificaciones de las armas en el ranking, MVP y simulaciones.
- Cómo resolver errores de compilación y organizar los archivos del proyecto.

## 4. Sugerencias aceptadas

Acepté utilizar controladores, DTOs, interfaces de servicios e inyección de dependencias para organizar la aplicación.

También utilicé `AppDbContext` y Entity Framework Core para almacenar personajes, cartas, eventos, usuarios y armas en MySQL.

Para la protección de datos, utilicé `PasswordHasher` para las contraseñas y AES-GCM para cifrar la ubicación secreta de los personajes.

En el sistema de armas, implementé una relación muchos a muchos mediante `PersonajeArma`, respetando la propuesta de catálogo de armas reutilizables y sus restricciones de compatibilidad.

## 5. Sugerencias corregidas y motivos

Durante el desarrollo encontré varios problemas en las sugerencias de ChatGPT que tuve que revisar y corregir.

**Método `ConvertirDto`:** se sugirió utilizar este método sin haberlo implementado completamente. Lo agregué para convertir las entidades de personajes en DTOs y permitir que la ubicación secreta se devolviera descifrada.

**Endpoints antiguos:** después de modificar los servicios, los endpoints de la Minimal API provocaban errores porque utilizaban métodos anteriores. Los retiré cuando sus funcionalidades quedaron disponibles mediante controladores.

**Migración del sistema de armas:** la migración inicial no incluía las tablas de armas. Creé una migración adicional llamada `SistemaArmas`, sin eliminar las tablas existentes.

**Resultados de los servicios:** al cambiar los métodos de armas para devolver resultados específicos en lugar de valores booleanos, aparecieron errores de compatibilidad entre interfaces, servicios y controladores. Corregí las declaraciones y las respuestas HTTP.

**Rutas duplicadas:** durante una modificación del controlador de armas quedaron atributos de rutas repetidos. Los corregí para evitar conflictos al ejecutar las peticiones.

**Enumeraciones:** el archivo `Enums` perdió su extensión `.cs`, causando errores porque no se encontraban `Faccion` y `EstadoPersonaje`. Corregí el nombre del archivo.

**Eventos y muertes:** revisé el orden en que se guardaban las participaciones y se actualizaban los estados de los personajes, para mantener la coherencia de los datos en MySQL.

**Bonificaciones de armas:** inicialmente las armas no afectaban al ranking ni a las batallas. Modifiqué las consultas para utilizar el poder efectivo sin alterar el poder base de las cartas.

Estas correcciones fueron necesarias para mantener la coherencia entre el código, los requisitos del proyecto original y el componente adicional.

## 6. Verificación del funcionamiento

Utilicé `dotnet build` para detectar errores de compilación y corregirlos.

Realicé pruebas mediante Swagger para comprobar las operaciones de personajes, cartas, eventos y armas, así como las reglas de compatibilidad, los cálculos de poder y las restricciones de muerte.

También utilicé MySQL Workbench para revisar las tablas, las relaciones y los datos almacenados.

## 7. Reflexión sobre el uso de IA

ChatGPT fue una herramienta de apoyo durante el desarrollo, pero sus respuestas no siempre fueron correctas o compatibles con el código existente.

Por eso fue necesario revisar los archivos, comparar las sugerencias con las instrucciones del docente y corregir los problemas encontrados.

El proceso me permitió practicar el uso de controladores, servicios, DTOs, relaciones entre entidades, migraciones, validaciones y protección de datos en una Web API.