using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Catalogo.Enums;
using BurgerPOS.Domain.Cobro.Entities;
using BurgerPOS.Domain.Cobro.Enums;
using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Domain.Turno.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Infrastructure.Services;

public class CobroService : ICobroService
{
    private readonly BurgerPosDbContext _context;

    public CobroService(BurgerPosDbContext context) => _context = context;

    public async Task<Orden> CrearOrdenAsync(ModalidadServicio modalidad, Guid operadorId, Guid? mesaId = null)
    {
        var orden = Orden.Crear(modalidad, operadorId, mesaId);
        _context.Ordenes.Add(orden);
        await _context.SaveChangesAsync();
        return orden;
    }

    public async Task<LineaOrden> AgregarLineaAsync(Guid ordenId, Guid productoId, int cantidad,
        decimal precioUnitario, string? notaLibre = null)
    {
        var orden = await _context.Ordenes.Include(o => o.Lineas).FirstOrDefaultAsync(o => o.Id == ordenId)
            ?? throw new KeyNotFoundException($"Orden {ordenId} no encontrada.");
        var linea = LineaOrden.CrearParaProducto(ordenId, productoId, cantidad, precioUnitario, notaLibre);
        _context.LineasOrden.Add(linea);
        orden.RecalcularSubtotal();
        await _context.SaveChangesAsync();
        return linea;
    }

    public async Task<Venta> CobrarAsync(Guid ordenId, Guid operadorId, Guid turnoCajaId,
        MetodoPago metodoPago, decimal propina = 0,
        decimal montoRecibido = 0, decimal? comisionBancaria = null,
        Guid? campaniaDescuentoId = null)
    {
        var orden = await _context.Ordenes.Include(o => o.Lineas).FirstOrDefaultAsync(o => o.Id == ordenId)
            ?? throw new KeyNotFoundException($"Orden {ordenId} no encontrada.");

        orden.RecalcularSubtotal();

        decimal montoDescuento = 0;
        if (campaniaDescuentoId.HasValue)
        {
            var campania = await _context.CampaniasDescuento.FindAsync(campaniaDescuentoId.Value);
            if (campania is not null && campania.EstaVigente(DateTime.UtcNow))
            {
                montoDescuento = campania.Tipo == TipoDescuento.Porcentaje
                    ? Math.Round(orden.Subtotal * campania.Valor / 100, 2)
                    : campania.Valor;
            }
        }

        var venta = Venta.Crear(ordenId, operadorId, turnoCajaId, orden.Subtotal, montoDescuento, propina);
        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync(); // folio asignado por la secuencia de Postgres aquí

        var pago = metodoPago == MetodoPago.Efectivo
            ? Pago.CrearEfectivo(venta.Id, montoRecibido, venta.Total)
            : Pago.CrearTarjeta(venta.Id, comisionBancaria);
        _context.Pagos.Add(pago);

        if (campaniaDescuentoId.HasValue && montoDescuento > 0)
            _context.DescuentosAplicados.Add(DescuentoAplicado.Crear(campaniaDescuentoId.Value, venta.Id, montoDescuento));

        var snapshot = $"FOLIO:{venta.Folio}|TOTAL:{venta.Total:F2}|FECHA:{venta.FechaCobro:dd/MM/yyyy HH:mm}";
        _context.Tickets.Add(Ticket.Crear(venta.Id, snapshot));

        orden.MarcarCobrada();
        await _context.SaveChangesAsync();
        return venta;
    }

    public async Task<Venta> AnularVentaAsync(Guid ventaId, Guid solicitanteId, bool esAdmin)
    {
        if (!esAdmin)
            throw new UnauthorizedAccessException("Solo el administrador puede anular ventas (RN-03).");
        var venta = await _context.Ventas.FindAsync(ventaId)
            ?? throw new KeyNotFoundException($"Venta {ventaId} no encontrada.");
        venta.Anular();
        await _context.SaveChangesAsync();
        return venta;
    }

    public async Task<TurnoCaja> AbrirTurnoAsync(Guid operadorId, decimal fondoInicial)
    {
        var turno = TurnoCaja.Abrir(operadorId, fondoInicial);
        turno.IniciarOperacion();
        _context.TurnosCaja.Add(turno);
        await _context.SaveChangesAsync();
        return turno;
    }
}
