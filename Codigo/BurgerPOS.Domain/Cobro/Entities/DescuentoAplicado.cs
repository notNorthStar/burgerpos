namespace BurgerPOS.Domain.Cobro.Entities;

public class DescuentoAplicado
{
    public Guid Id { get; private set; }
    public Guid CampaniaId { get; private set; }
    public Guid VentaId { get; private set; }
    public decimal MontoCalculado { get; private set; }

    private DescuentoAplicado() { }

    public static DescuentoAplicado Crear(Guid campaniaId, Guid ventaId, decimal montoCalculado)
    {
        return new DescuentoAplicado
        {
            Id = Guid.NewGuid(),
            CampaniaId = campaniaId,
            VentaId = ventaId,
            MontoCalculado = montoCalculado
        };
    }
}
