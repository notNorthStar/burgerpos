namespace BurgerPOS.Domain.Catalogo.Entities;

public class Combo
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public decimal PrecioEspecial { get; private set; }
    public bool Activo { get; private set; }

    public ICollection<ComponenteCombo> Componentes { get; private set; } = new List<ComponenteCombo>();

    private Combo() { }

    public static Combo Crear(string nombre, decimal precioEspecial)
    {
        return new Combo
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            PrecioEspecial = precioEspecial,
            Activo = true
        };
    }

    public decimal CalcularAhorro()
        => Componentes.Sum(c => c.Producto?.PrecioBase * c.Cantidad ?? 0) - PrecioEspecial;

    public void Desactivar() => Activo = false;
}
