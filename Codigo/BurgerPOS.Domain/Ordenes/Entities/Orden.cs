using BurgerPOS.Domain.Ordenes.Enums;

namespace BurgerPOS.Domain.Ordenes.Entities;

public class Orden
{
    public Guid Id { get; private set; }
    public int FolioOrden { get; private set; }
    public ModalidadServicio Modalidad { get; private set; }
    public Guid? MesaId { get; private set; }
    public EstadoOrden Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public Guid OperadorId { get; private set; }
    public decimal Subtotal { get; private set; }
    public int TotalEnvios { get; private set; } = 0;

    public Mesa? Mesa { get; private set; }
    public ICollection<LineaOrden> Lineas { get; private set; } = new List<LineaOrden>();

    private Orden() { }

    public static Orden Crear(ModalidadServicio modalidad, Guid operadorId, Guid? mesaId = null)
    {
        return new Orden
        {
            Id = Guid.NewGuid(),
            Modalidad = modalidad,
            MesaId = mesaId,
            Estado = EstadoOrden.Borrador,
            FechaCreacion = DateTime.UtcNow,
            OperadorId = operadorId,
            Subtotal = 0
        };
    }

    public void RecalcularSubtotal()
    {
        Subtotal = Lineas.Sum(l => l.CalcularImporte());
    }

    public void EnviarACocina()
    {
        if (Estado != EstadoOrden.Borrador)
            throw new InvalidOperationException("Solo se puede enviar a cocina una orden en borrador.");
        TotalEnvios++;
        Estado = EstadoOrden.EnviadaACocina;
    }

    public void ReenviarACocina()
    {
        if (Estado == EstadoOrden.Cobrada || Estado == EstadoOrden.Cancelada)
            throw new InvalidOperationException("No se puede reenviar una orden cobrada o cancelada.");
        TotalEnvios++;
        Estado = EstadoOrden.EnviadaACocina;
    }

    public void MarcarEnPreparacion()
    {
        if (Estado != EstadoOrden.EnviadaACocina)
            throw new InvalidOperationException("La orden no está enviada a cocina.");
        Estado = EstadoOrden.EnPreparacion;
    }

    public void MarcarLista()
    {
        if (Estado != EstadoOrden.EnPreparacion)
            throw new InvalidOperationException("La orden no está en preparación.");
        Estado = EstadoOrden.Lista;
    }

    public void Entregar()
    {
        if (Estado != EstadoOrden.Lista)
            throw new InvalidOperationException("La orden no está lista para entrega.");
        Estado = EstadoOrden.Entregada;
    }

    public void MarcarCobrada()
    {
        Estado = EstadoOrden.Cobrada;
    }

    public void Cancelar()
    {
        if (Estado == EstadoOrden.Cobrada)
            throw new InvalidOperationException("No se puede cancelar una orden ya cobrada.");
        Estado = EstadoOrden.Cancelada;
    }
}
