using BurgerPOS.Domain.Cobro.Enums;

namespace BurgerPOS.Domain.Cobro.Entities;

public class Pago
{
    public Guid Id { get; private set; }
    public Guid VentaId { get; private set; }
    public MetodoPago Metodo { get; private set; }
    public decimal? MontoRecibido { get; private set; }
    public decimal? Cambio { get; private set; }
    public decimal? ComisionBancaria { get; private set; }

    public Venta? Venta { get; private set; }

    private Pago() { }

    public static Pago CrearEfectivo(Guid ventaId, decimal montoRecibido, decimal total)
    {
        return new Pago
        {
            Id = Guid.NewGuid(),
            VentaId = ventaId,
            Metodo = MetodoPago.Efectivo,
            MontoRecibido = montoRecibido,
            Cambio = Math.Round(montoRecibido - total, 2)
        };
    }

    public static Pago CrearTarjeta(Guid ventaId, decimal? comisionBancaria = null)
    {
        return new Pago
        {
            Id = Guid.NewGuid(),
            VentaId = ventaId,
            Metodo = MetodoPago.Tarjeta,
            ComisionBancaria = comisionBancaria
        };
    }
}
