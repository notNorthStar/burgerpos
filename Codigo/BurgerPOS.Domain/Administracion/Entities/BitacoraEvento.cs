using BurgerPOS.Domain.Administracion.Enums;
using BurgerPOS.Domain.Identidad.Enums;

namespace BurgerPOS.Domain.Administracion.Entities;

public class BitacoraEvento
{
    public Guid Id { get; private set; }
    public DateTime Timestamp { get; private set; }
    public Guid UsuarioId { get; private set; }
    public RolOperativo? RolOperativoDeclarado { get; private set; }
    public TipoEvento Tipo { get; private set; }
    public string EntidadAfectada { get; private set; } = string.Empty;
    public Guid? EntidadId { get; private set; }
    public string? ValorAnterior { get; private set; }
    public string? ValorNuevo { get; private set; }
    public string IpOrigen { get; private set; } = string.Empty;

    private BitacoraEvento() { }

    public static BitacoraEvento Registrar(Guid usuarioId, TipoEvento tipo, string entidadAfectada,
        string ipOrigen, Guid? entidadId = null, string? valorAnterior = null,
        string? valorNuevo = null, RolOperativo? rolOperativo = null)
    {
        return new BitacoraEvento
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            UsuarioId = usuarioId,
            Tipo = tipo,
            EntidadAfectada = entidadAfectada,
            IpOrigen = ipOrigen,
            EntidadId = entidadId,
            ValorAnterior = valorAnterior,
            ValorNuevo = valorNuevo,
            RolOperativoDeclarado = rolOperativo
        };
    }
}
