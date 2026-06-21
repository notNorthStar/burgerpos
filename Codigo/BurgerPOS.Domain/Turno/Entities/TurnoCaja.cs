using BurgerPOS.Domain.Turno.Enums;

namespace BurgerPOS.Domain.Turno.Entities;

public class TurnoCaja
{
    public Guid Id { get; private set; }
    public decimal FondoInicial { get; private set; }
    public DateTime FechaApertura { get; private set; }
    public DateTime? FechaCierre { get; private set; }
    public Guid OperadorAperturaId { get; private set; }
    public Guid? OperadorCierreId { get; private set; }
    public decimal EfectivoEsperado { get; private set; }
    public decimal? EfectivoContado { get; private set; }
    public decimal? Diferencia { get; private set; }
    public EstadoTurno Estado { get; private set; }
    public string? Observaciones { get; private set; }

    private TurnoCaja() { }

    public static TurnoCaja Abrir(Guid operadorAperturaId, decimal fondoInicial)
    {
        return new TurnoCaja
        {
            Id = Guid.NewGuid(),
            FondoInicial = fondoInicial,
            FechaApertura = DateTime.UtcNow,
            OperadorAperturaId = operadorAperturaId,
            EfectivoEsperado = fondoInicial,
            Estado = EstadoTurno.Abierto
        };
    }

    public void IniciarOperacion() => Estado = EstadoTurno.EnOperacion;

    public void SumarEfectivoEsperado(decimal monto) => EfectivoEsperado += monto;

    public void Arquear(decimal contado, Guid operadorCierreId)
    {
        EfectivoContado = contado;
        Diferencia = contado - EfectivoEsperado;
        OperadorCierreId = operadorCierreId;
        Estado = EstadoTurno.ArqueoEnProceso;
    }

    public void Cerrar(string? observaciones = null)
    {
        if (Estado != EstadoTurno.ArqueoEnProceso)
            throw new InvalidOperationException("Debe realizarse el arqueo antes de cerrar.");
        Estado = EstadoTurno.Cerrado;
        FechaCierre = DateTime.UtcNow;
        Observaciones = observaciones;
    }
}
