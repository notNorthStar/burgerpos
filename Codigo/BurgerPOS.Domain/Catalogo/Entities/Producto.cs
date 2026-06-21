namespace BurgerPOS.Domain.Catalogo.Entities;

public class Producto
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public decimal PrecioBase { get; private set; }
    public Guid CategoriaId { get; private set; }
    public bool Disponible { get; private set; }
    public bool Activo { get; private set; }

    public Categoria? Categoria { get; private set; }
    public Receta? Receta { get; private set; }
    public ICollection<Modificador> Modificadores { get; private set; } = new List<Modificador>();

    private Producto() { }

    public static Producto Crear(string nombre, string descripcion, decimal precioBase, Guid categoriaId)
    {
        return new Producto
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Descripcion = descripcion,
            PrecioBase = precioBase,
            CategoriaId = categoriaId,
            Disponible = true,
            Activo = true
        };
    }

    public void ActualizarPrecio(decimal nuevoPrecio) => PrecioBase = nuevoPrecio;
    public void ActualizarNombre(string nombre) => Nombre = nombre;
    public void ActualizarDescripcion(string descripcion) => Descripcion = descripcion;

    public void MarcarNoDisponible() => Disponible = false;
    public void MarcarDisponible() => Disponible = true;
    public void Desactivar() => Activo = false;

    public decimal CalcularPrecioConModificadores(IEnumerable<Modificador> mods)
        => PrecioBase + mods.Sum(m => m.DeltaPrecio);
}
