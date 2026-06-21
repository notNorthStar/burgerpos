namespace BurgerPOS.Domain.Inventario.Entities;

public class AjusteSaldo
{
    public Guid Id { get; private set; }
    public Guid InsumoId { get; private set; }
    public decimal CantidadAnterior { get; private set; }
    public decimal CantidadNueva { get; private set; }
    public decimal Diferencia { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public Guid AdminId { get; private set; }
    public DateTime Fecha { get; private set; }

    public Insumo? Insumo { get; private set; }

    private AjusteSaldo() { }

    public static AjusteSaldo Crear(Guid insumoId, decimal cantidadAnterior, decimal cantidadNueva,
        string motivo, Guid adminId)
    {
        return new AjusteSaldo
        {
            Id = Guid.NewGuid(),
            InsumoId = insumoId,
            CantidadAnterior = cantidadAnterior,
            CantidadNueva = cantidadNueva,
            Diferencia = cantidadNueva - cantidadAnterior,
            Motivo = motivo,
            AdminId = adminId,
            Fecha = DateTime.UtcNow
        };
    }
}
