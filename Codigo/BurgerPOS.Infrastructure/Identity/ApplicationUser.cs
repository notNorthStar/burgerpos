using BurgerPOS.Domain.Identidad.Enums;
using Microsoft.AspNetCore.Identity;

namespace BurgerPOS.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string NombreCompleto { get; set; } = string.Empty;
    public Rol Rol { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
}
