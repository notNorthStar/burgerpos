namespace BurgerPOS.Domain.Inventario.Entities;

public class EntradaInventario
{
    public Guid Id { get; private set; }
    public Guid InsumoId { get; private set; }
    public decimal Cantidad { get; private set; }
    public Guid UsuarioId { get; private set; }
    public DateTime Fecha { get; private set; }
    public string? Proveedor { get; private set; }
    public decimal? Costo { get; private set; }

    public Insumo? Insumo { get; private set; }

    private EntradaInventario() { }

    public static EntradaInventario Crear(Guid insumoId, decimal cantidad, Guid usuarioId,
        string? proveedor = null, decimal? costo = null)
    {
        return new EntradaInventario
        {
            Id = Guid.NewGuid(),
            InsumoId = insumoId,
            Cantidad = cantidad,
            UsuarioId = usuarioId,
            Fecha = DateTime.UtcNow,
            Proveedor = proveedor,
            Costo = costo
        };
    }
}
