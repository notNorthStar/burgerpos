using BurgerPOS.Domain.Inventario.Entities;
using BurgerPOS.Domain.Inventario.Enums;

namespace BurgerPOS.Application.Interfaces.Services;

public interface IInventarioService
{
    Task<Insumo> CrearInsumoAsync(string nombre, UnidadMedida unidad, decimal saldoInicial, decimal nivelAlerta);
    Task<Insumo?> ObtenerInsumoAsync(Guid id);
    Task<List<Insumo>> ObtenerInsumosActivosAsync();

    Task<EntradaInventario> RegistrarEntradaAsync(Guid insumoId, decimal cantidad, Guid usuarioId,
        string? proveedor = null, decimal? costo = null);

    Task<Merma> RegistrarMermaAsync(Guid insumoId, decimal cantidad, MotivoMerma motivo,
        Guid usuarioId, string? descripcion = null);

    Task<AjusteSaldo> AjustarSaldoAsync(Guid insumoId, decimal nuevaCantidad, string motivo, Guid adminId);

    Task DescontarPorVentaAsync(IEnumerable<(Guid productoId, int cantidad)> lineasVendidas);
}
