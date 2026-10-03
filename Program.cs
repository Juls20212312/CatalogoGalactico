using System.Text.Json.Serialization;
using CatalogoGalactico.Data;
using CatalogoGalactico.Services;
using CatalogoGalactico.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DataStore>();
builder.Services.AddScoped<PersonajeService>();
builder.Services.AddScoped<CardService>();
builder.Services.AddScoped<EventoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Catálogo Galáctico funcionando");

app.MapPersonajeEndpoints();
app.MapCardEndpoints();
app.MapEventoEndpoints();

app.Run();