using BurgerPOS.Domain.Inventario.Enums;

namespace BurgerPOS.Domain.Inventario.Entities;

public class Merma
{
    public Guid Id { get; private set; }
    public Guid InsumoId { get; private set; }
    public decimal Cantidad { get; private set; }
    public MotivoMerma Motivo { get; private set; }
    public string? Descripcion { get; private set; }
    public Guid UsuarioId { get; private set; }
    public DateTime Fecha { get; private set; }

    public Insumo? Insumo { get; private set; }

    private Merma() { }

    public static Merma Crear(Guid insumoId, decimal cantidad, MotivoMerma motivo,
        Guid usuarioId, string? descripcion = null)
    {
        return new Merma
        {
            Id = Guid.NewGuid(),
            InsumoId = insumoId,
            Cantidad = cantidad,
            Motivo = motivo,
            Descripcion = descripcion,
            UsuarioId = usuarioId,
            Fecha = DateTime.UtcNow
        };
    }
}
