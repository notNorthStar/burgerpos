using BurgerPOS.Domain.Inventario.Enums;

namespace BurgerPOS.Domain.Inventario.Entities;

public class Insumo
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public UnidadMedida Unidad { get; private set; }
    public decimal SaldoActual { get; private set; }
    public decimal NivelAlerta { get; private set; }
    public bool Activo { get; private set; }

    private Insumo() { }

    public static Insumo Crear(string nombre, UnidadMedida unidad, decimal saldoInicial, decimal nivelAlerta)
    {
        return new Insumo
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Unidad = unidad,
            SaldoActual = saldoInicial,
            NivelAlerta = nivelAlerta,
            Activo = true
        };
    }

    public void Descontar(decimal cantidad)
    {
        if (cantidad <= 0) throw new ArgumentException("La cantidad a descontar debe ser mayor a cero.");
        if (cantidad > SaldoActual) throw new InvalidOperationException($"Saldo insuficiente de '{Nombre}'. Disponible: {SaldoActual}, requerido: {cantidad}.");
        SaldoActual -= cantidad;
    }

    public void Incrementar(decimal cantidad)
    {
        if (cantidad <= 0) throw new ArgumentException("La cantidad a incrementar debe ser mayor a cero.");
        SaldoActual += cantidad;
    }

    public void AjustarSaldo(decimal nuevaCantidad) => SaldoActual = nuevaCantidad;

    public bool EstaEnAlerta() => SaldoActual <= NivelAlerta;

    public void ActualizarNivelAlerta(decimal nivel) => NivelAlerta = nivel;
    public void Desactivar() => Activo = false;
}
