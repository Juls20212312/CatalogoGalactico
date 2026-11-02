using CatalogoGalactico.Data;
using CatalogoGalactico.Dtos;
using CatalogoGalactico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalactico.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(
        AppDbContext db,
        IPasswordHasher<Usuario> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioDto?> CrearAsync(UsuarioCrearDto dto)
    {
        string email = dto.Email.Trim();

        bool existe = await _db.Usuarios
            .AnyAsync(u => u.Email == email);

        if (existe)
        {
            return null;
        }

        var usuario = new Usuario
        {
            Nombre = dto.Nombre.Trim(),
            Email = email
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(
            usuario,
            dto.Password
        );

        _db.Usuarios.Add(usuario);

        await _db.SaveChangesAsync();

        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email
        };
    }
}