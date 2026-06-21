namespace BurgerPOS.Domain.Ordenes.Entities;

public class LineaModificador
{
    public Guid LineaOrdenId { get; private set; }
    public Guid ModificadorId { get; private set; }
    public decimal DeltaAplicado { get; private set; }

    public LineaOrden? LineaOrden { get; private set; }

    private LineaModificador() { }

    public static LineaModificador Crear(Guid lineaOrdenId, Guid modificadorId, decimal deltaAplicado)
    {
        return new LineaModificador
        {
            LineaOrdenId = lineaOrdenId,
            ModificadorId = modificadorId,
            DeltaAplicado = deltaAplicado
        };
    }
}
