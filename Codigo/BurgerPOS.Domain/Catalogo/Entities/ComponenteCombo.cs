namespace BurgerPOS.Domain.Catalogo.Entities;

public class ComponenteCombo
{
    public Guid ComboId { get; private set; }
    public Guid ProductoId { get; private set; }
    public int Cantidad { get; private set; }

    public Combo? Combo { get; private set; }
    public Producto? Producto { get; private set; }

    private ComponenteCombo() { }

    public static ComponenteCombo Crear(Guid comboId, Guid productoId, int cantidad)
    {
        return new ComponenteCombo
        {
            ComboId = comboId,
            ProductoId = productoId,
            Cantidad = cantidad
        };
    }
}
