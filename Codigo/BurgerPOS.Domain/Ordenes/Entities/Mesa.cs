using BurgerPOS.Domain.Ordenes.Enums;

namespace BurgerPOS.Domain.Ordenes.Entities;

public class Mesa
{
    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public EstadoMesa Estado { get; private set; }

    private Mesa() { }

    public static Mesa Crear(int numero)
    {
        return new Mesa
        {
            Id = Guid.NewGuid(),
            Numero = numero,
            Estado = EstadoMesa.Libre
        };
    }

    public void Ocupar() => Estado = EstadoMesa.Ocupada;
    public void AbrirCuenta() => Estado = EstadoMesa.ConCuentaAbierta;
    public void MarcarPorLimpiar() => Estado = EstadoMesa.PorLimpiar;
    public void Liberar() => Estado = EstadoMesa.Libre;
}
