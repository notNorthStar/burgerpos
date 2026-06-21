namespace BurgerPOS.Domain.Catalogo.Entities;

public class Receta
{
    public Guid Id { get; private set; }
    public Guid ProductoId { get; private set; }

    public Producto? Producto { get; private set; }
    public ICollection<LineaReceta> Lineas { get; private set; } = new List<LineaReceta>();

    private Receta() { }

    public static Receta Crear(Guid productoId)
    {
        return new Receta
        {
            Id = Guid.NewGuid(),
            ProductoId = productoId
        };
    }

    public void AgregarLinea(Guid insumoId, decimal cantidad)
    {
        Lineas.Add(LineaReceta.Crear(Id, insumoId, cantidad));
    }
}
