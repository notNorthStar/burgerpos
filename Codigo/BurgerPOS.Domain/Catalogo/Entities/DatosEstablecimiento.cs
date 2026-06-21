namespace BurgerPOS.Domain.Catalogo.Entities;

public class DatosEstablecimiento
{
    public Guid Id { get; private set; }
    public string NombreComercial { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string? Rfc { get; private set; }
    public string? LeyendaTicket { get; private set; }
    public DateTime FechaActualizacion { get; private set; }

    private DatosEstablecimiento() { }

    public static DatosEstablecimiento Crear(string nombreComercial, string direccion, string telefono,
        string? rfc = null, string? leyendaTicket = null)
    {
        return new DatosEstablecimiento
        {
            Id = Guid.NewGuid(),
            NombreComercial = nombreComercial,
            Direccion = direccion,
            Telefono = telefono,
            Rfc = rfc,
            LeyendaTicket = leyendaTicket,
            FechaActualizacion = DateTime.UtcNow
        };
    }

    public void Actualizar(string nombreComercial, string direccion, string telefono,
        string? rfc = null, string? leyendaTicket = null)
    {
        NombreComercial = nombreComercial;
        Direccion = direccion;
        Telefono = telefono;
        Rfc = rfc;
        LeyendaTicket = leyendaTicket;
        FechaActualizacion = DateTime.UtcNow;
    }
}
