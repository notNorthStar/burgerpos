namespace BurgerPOS.Domain.Catalogo.Entities;

public class Modificador
{
    public Guid Id { get; private set; }
    public Guid ProductoId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public decimal DeltaPrecio { get; private set; }

    public Producto? Producto { get; private set; }

    private Modificador() { }

    public static Modificador Crear(Guid productoId, string nombre, decimal deltaPrecio)
    {
        return new Modificador
        {
            Id = Guid.NewGuid(),
            ProductoId = productoId,
            Nombre = nombre,
            DeltaPrecio = deltaPrecio
        };
    }
}
