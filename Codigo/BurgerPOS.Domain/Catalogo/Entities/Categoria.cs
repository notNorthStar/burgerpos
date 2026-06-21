namespace BurgerPOS.Domain.Catalogo.Entities;

public class Categoria
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public int OrdenVisual { get; private set; }

    private Categoria() { }

    public static Categoria Crear(string nombre, int ordenVisual = 0)
    {
        return new Categoria
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            OrdenVisual = ordenVisual
        };
    }

    public void ActualizarNombre(string nombre) => Nombre = nombre;
    public void ActualizarOrden(int orden) => OrdenVisual = orden;
}
