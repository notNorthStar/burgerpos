using BurgerPOS.Domain.Identidad.Enums;

namespace BurgerPOS.Domain.Identidad.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string NombreUsuario { get; private set; } = string.Empty;
    public string NombreCompleto { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public Rol Rol { get; private set; }
    public bool Activo { get; private set; }
    public DateTime FechaAlta { get; private set; }

    private Usuario() { }

    public static Usuario Crear(string nombreUsuario, string nombreCompleto, string passwordHash, Rol rol)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            NombreUsuario = nombreUsuario,
            NombreCompleto = nombreCompleto,
            PasswordHash = passwordHash,
            Rol = rol,
            Activo = true,
            FechaAlta = DateTime.UtcNow
        };
    }

    public void CambiarPassword(string nuevoHash)
    {
        PasswordHash = nuevoHash;
    }

    public void Desactivar()
    {
        Activo = false;
    }
}
