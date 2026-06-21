using BurgerPOS.Domain.Catalogo.Enums;

namespace BurgerPOS.Domain.Catalogo.Entities;

public class CampaniaDescuento
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public TipoDescuento Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public AlcanceDescuento Alcance { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public List<int> DiasSemana { get; private set; } = new();
    public TimeSpan? HoraInicio { get; private set; }
    public TimeSpan? HoraFin { get; private set; }
    public EstadoCampania Estado { get; private set; }

    private CampaniaDescuento() { }

    public static CampaniaDescuento Crear(
        string nombre, TipoDescuento tipo, decimal valor, AlcanceDescuento alcance,
        DateTime fechaInicio, DateTime fechaFin,
        List<int>? diasSemana = null,
        TimeSpan? horaInicio = null, TimeSpan? horaFin = null)
    {
        return new CampaniaDescuento
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Tipo = tipo,
            Valor = valor,
            Alcance = alcance,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            DiasSemana = diasSemana ?? new List<int> { 0, 1, 2, 3, 4, 5, 6 },
            HoraInicio = horaInicio,
            HoraFin = horaFin,
            Estado = EstadoCampania.Borrador
        };
    }

    public bool EstaVigente(DateTime momento)
    {
        if (Estado != EstadoCampania.Activa) return false;
        if (momento < FechaInicio || momento > FechaFin) return false;
        if (!DiasSemana.Contains((int)momento.DayOfWeek)) return false;
        if (HoraInicio.HasValue && momento.TimeOfDay < HoraInicio) return false;
        if (HoraFin.HasValue && momento.TimeOfDay > HoraFin) return false;
        return true;
    }

    public void Activar() => Estado = EstadoCampania.Activa;
    public void Pausar() => Estado = EstadoCampania.Pausada;
    public void Expirar() => Estado = EstadoCampania.Expirada;
}
