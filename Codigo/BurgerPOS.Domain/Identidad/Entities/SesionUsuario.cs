using BurgerPOS.Domain.Identidad.Enums;

namespace BurgerPOS.Domain.Identidad.Entities;

public class SesionUsuario
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public RolOperativo RolOperativo { get; private set; }
    public DateTime Inicio { get; private set; }
    public DateTime? Fin { get; private set; }

    public Usuario? Usuario { get; private set; }

    private SesionUsuario() { }

    public static SesionUsuario Abrir(Guid usuarioId, RolOperativo rolOperativo)
    {
        return new SesionUsuario
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            RolOperativo = rolOperativo,
            Inicio = DateTime.UtcNow
        };
    }

    public void Cerrar()
    {
        Fin = DateTime.UtcNow;
    }
}
