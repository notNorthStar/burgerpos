namespace BurgerPOS.Domain.Catalogo.Entities;

public class LineaReceta
{
    public Guid RecetaId { get; private set; }
    public Guid InsumoId { get; private set; }
    public decimal Cantidad { get; private set; }

    private LineaReceta() { }

    public static LineaReceta Crear(Guid recetaId, Guid insumoId, decimal cantidad)
    {
        return new LineaReceta
        {
            RecetaId = recetaId,
            InsumoId = insumoId,
            Cantidad = cantidad
        };
    }
}
