using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Cocina;

[Authorize]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    private readonly IInventarioService _inventario;

    public IndexModel(BurgerPosDbContext db, IInventarioService inventario)
    {
        _db = db;
        _inventario = inventario;
    }

    public List<Orden> Ordenes { get; set; } = [];
    public Dictionary<Guid, string> NombresProducto { get; set; } = [];
    public HashSet<Guid> IdsComplemento { get; set; } = [];

    public async Task OnGetAsync()
    {
        Ordenes = await _db.Ordenes
            .Include(o => o.Lineas)
            .Include(o => o.Mesa)
            .Where(o => o.Estado == EstadoOrden.EnviadaACocina || o.Estado == EstadoOrden.EnPreparacion)
            .OrderBy(o => o.FechaCreacion)
            .ToListAsync();

        var ids = Ordenes.SelectMany(o => o.Lineas)
            .Where(l => l.ProductoId.HasValue)
            .Select(l => l.ProductoId!.Value)
            .Distinct()
            .ToList();

        var productos = await _db.Productos
            .Include(p => p.Categoria)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        NombresProducto = productos.ToDictionary(p => p.Id, p => p.Nombre);

        IdsComplemento = productos
            .Where(p => p.Categoria != null &&
                        p.Categoria.Nombre.Contains("complemento", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Id)
            .ToHashSet();
    }

    public async Task<IActionResult> OnPostIniciarAsync(Guid ordenId)
    {
        var orden = await _db.Ordenes.FindAsync(ordenId);
        if (orden?.Estado == EstadoOrden.EnviadaACocina)
        {
            orden.MarcarEnPreparacion();
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostListaAsync(Guid ordenId)
    {
        var orden = await _db.Ordenes.Include(o => o.Lineas).FirstOrDefaultAsync(o => o.Id == ordenId);
        if (orden is not null)
        {
            if (orden.Estado == EstadoOrden.EnviadaACocina)
                orden.MarcarEnPreparacion();
            if (orden.Estado == EstadoOrden.EnPreparacion)
                orden.MarcarLista();

            await _db.SaveChangesAsync();

            // Descontar insumos solo de las líneas del lote recién preparado
            var lineasLote = orden.Lineas
                .Where(l => l.ProductoId.HasValue &&
                            (orden.TotalEnvios == 0 || l.NumeroEnvio == orden.TotalEnvios))
                .Select(l => (l.ProductoId!.Value, l.Cantidad));

            await _inventario.DescontarPorVentaAsync(lineasLote);
        }
        return RedirectToPage();
    }
}
