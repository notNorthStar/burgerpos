namespace BurgerPOS.Domain.Cobro.Entities;

public class Ticket
{
    public Guid Id { get; private set; }
    public Guid VentaId { get; private set; }
    public string ContenidoSnapshot { get; private set; } = string.Empty;
    public DateTime FechaEmision { get; private set; }

    public Venta? Venta { get; private set; }

    private Ticket() { }

    public static Ticket Crear(Guid ventaId, string contenidoSnapshot)
    {
        return new Ticket
        {
            Id = Guid.NewGuid(),
            VentaId = ventaId,
            ContenidoSnapshot = contenidoSnapshot,
            FechaEmision = DateTime.UtcNow
        };
    }
}
