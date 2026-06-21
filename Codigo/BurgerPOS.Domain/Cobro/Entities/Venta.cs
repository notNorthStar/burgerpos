namespace BurgerPOS.Domain.Cobro.Entities;

public class Venta
{
    public Guid Id { get; private set; }
    public int Folio { get; private set; }
    public Guid OrdenId { get; private set; }
    public DateTime FechaCobro { get; private set; }
    public Guid OperadorId { get; private set; }
    public Guid TurnoCajaId { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal MontoDescuento { get; private set; }
    public decimal BaseGravable { get; private set; }
    public decimal Iva { get; private set; }
    public decimal Propina { get; private set; }
    public decimal Total { get; private set; }
    public bool Anulada { get; private set; }

    public Pago? Pago { get; private set; }
    public Ticket? Ticket { get; private set; }
    public DescuentoAplicado? DescuentoAplicado { get; private set; }

    private Venta() { }

    public static Venta Crear(Guid ordenId, Guid operadorId, Guid turnoCajaId,
        decimal subtotal, decimal montoDescuento, decimal propina)
    {
        const decimal tasaIva = 0.16m;
        var baseGravable = subtotal - montoDescuento;
        var iva = Math.Round(baseGravable * tasaIva, 2);
        var total = baseGravable + iva + propina;

        return new Venta
        {
            Id = Guid.NewGuid(),
            OrdenId = ordenId,
            FechaCobro = DateTime.UtcNow,
            OperadorId = operadorId,
            TurnoCajaId = turnoCajaId,
            Subtotal = subtotal,
            MontoDescuento = montoDescuento,
            BaseGravable = baseGravable,
            Iva = iva,
            Propina = propina,
            Total = total,
            Anulada = false
        };
    }

    public void Anular()
    {
        if (Anulada) throw new InvalidOperationException("La venta ya está anulada.");
        Anulada = true;
    }
}
