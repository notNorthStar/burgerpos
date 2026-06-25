namespace BurgerPOS.Domain.Ordenes.Entities;

public class LineaOrden
{
    public Guid Id { get; private set; }
    public Guid OrdenId { get; private set; }
    public Guid? ProductoId { get; private set; }
    public Guid? ComboId { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public string? NotaLibre { get; private set; }
    public int NumeroEnvio { get; private set; } = 0;

    public ICollection<LineaModificador> Modificadores { get; private set; } = new List<LineaModificador>();

    private LineaOrden() { }

    public static LineaOrden CrearParaProducto(Guid ordenId, Guid productoId, int cantidad,
        decimal precioUnitario, string? notaLibre = null)
    {
        return new LineaOrden
        {
            Id = Guid.NewGuid(),
            OrdenId = ordenId,
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
            NotaLibre = notaLibre
        };
    }

    public static LineaOrden CrearParaCombo(Guid ordenId, Guid comboId, int cantidad, decimal precioUnitario)
    {
        return new LineaOrden
        {
            Id = Guid.NewGuid(),
            OrdenId = ordenId,
            ComboId = comboId,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario
        };
    }

    public void MarcarEnvio(int numeroEnvio)
    {
        if (NumeroEnvio == 0) NumeroEnvio = numeroEnvio;
    }

    public decimal CalcularImporte()
        => Cantidad * (PrecioUnitario + Modificadores.Sum(m => m.DeltaAplicado));
}
