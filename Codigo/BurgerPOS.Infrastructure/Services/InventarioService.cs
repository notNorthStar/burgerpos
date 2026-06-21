using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Inventario.Entities;
using BurgerPOS.Domain.Inventario.Enums;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Infrastructure.Services;

public class InventarioService : IInventarioService
{
    private readonly BurgerPosDbContext _context;

    public InventarioService(BurgerPosDbContext context) => _context = context;

    public async Task<Insumo> CrearInsumoAsync(string nombre, UnidadMedida unidad,
        decimal saldoInicial, decimal nivelAlerta)
    {
        var insumo = Insumo.Crear(nombre, unidad, saldoInicial, nivelAlerta);
        _context.Insumos.Add(insumo);
        await _context.SaveChangesAsync();
        return insumo;
    }

    public async Task<Insumo?> ObtenerInsumoAsync(Guid id)
        => await _context.Insumos.FindAsync(id);

    public async Task<List<Insumo>> ObtenerInsumosActivosAsync()
        => await _context.Insumos.Where(i => i.Activo).OrderBy(i => i.Nombre).ToListAsync();

    public async Task<EntradaInventario> RegistrarEntradaAsync(Guid insumoId, decimal cantidad,
        Guid usuarioId, string? proveedor = null, decimal? costo = null)
    {
        var insumo = await _context.Insumos.FindAsync(insumoId)
            ?? throw new KeyNotFoundException($"Insumo {insumoId} no encontrado.");
        insumo.Incrementar(cantidad);
        var entrada = EntradaInventario.Crear(insumoId, cantidad, usuarioId, proveedor, costo);
        _context.EntradasInventario.Add(entrada);
        await _context.SaveChangesAsync();
        return entrada;
    }

    public async Task<Merma> RegistrarMermaAsync(Guid insumoId, decimal cantidad,
        MotivoMerma motivo, Guid usuarioId, string? descripcion = null)
    {
        var insumo = await _context.Insumos.FindAsync(insumoId)
            ?? throw new KeyNotFoundException($"Insumo {insumoId} no encontrado.");
        insumo.Descontar(cantidad);
        var merma = Merma.Crear(insumoId, cantidad, motivo, usuarioId, descripcion);
        _context.Mermas.Add(merma);
        await _context.SaveChangesAsync();
        return merma;
    }

    public async Task<AjusteSaldo> AjustarSaldoAsync(Guid insumoId, decimal nuevaCantidad,
        string motivo, Guid adminId)
    {
        var insumo = await _context.Insumos.FindAsync(insumoId)
            ?? throw new KeyNotFoundException($"Insumo {insumoId} no encontrado.");
        var ajuste = AjusteSaldo.Crear(insumoId, insumo.SaldoActual, nuevaCantidad, motivo, adminId);
        insumo.AjustarSaldo(nuevaCantidad);
        _context.AjustesSaldo.Add(ajuste);
        await _context.SaveChangesAsync();
        return ajuste;
    }

    public async Task DescontarPorVentaAsync(IEnumerable<(Guid productoId, int cantidad)> lineasVendidas)
    {
        foreach (var (productoId, cantidadVendida) in lineasVendidas)
        {
            var receta = await _context.Recetas
                .Include(r => r.Lineas)
                .FirstOrDefaultAsync(r => r.ProductoId == productoId);
            if (receta is null) continue;
            foreach (var linea in receta.Lineas)
            {
                var insumo = await _context.Insumos.FindAsync(linea.InsumoId);
                if (insumo is null) continue;
                var aDescontar = linea.Cantidad * cantidadVendida;
                if (aDescontar <= insumo.SaldoActual)
                    insumo.Descontar(aDescontar);
            }
        }
        await _context.SaveChangesAsync();
    }
}
