using BurgerPOS.Domain.Cobro.Entities;
using BurgerPOS.Domain.Cobro.Enums;
using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Domain.Turno.Entities;

namespace BurgerPOS.Application.Interfaces.Services;

public interface ICobroService
{
    Task<Orden> CrearOrdenAsync(ModalidadServicio modalidad, Guid operadorId, Guid? mesaId = null);

    Task<LineaOrden> AgregarLineaAsync(Guid ordenId, Guid productoId, int cantidad,
        decimal precioUnitario, string? notaLibre = null);

    Task<Venta> CobrarAsync(Guid ordenId, Guid operadorId, Guid turnoCajaId,
        MetodoPago metodoPago, decimal propina = 0,
        decimal montoRecibido = 0, decimal? comisionBancaria = null,
        Guid? campaniaDescuentoId = null);

    Task<Venta> AnularVentaAsync(Guid ventaId, Guid solicitanteId, bool esAdmin);

    Task<TurnoCaja> AbrirTurnoAsync(Guid operadorId, decimal fondoInicial);
}
