using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Admin.Reportes;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    public IndexModel(BurgerPosDbContext db) => _db = db;

    public DateTime Desde { get; set; } = DateTime.Today.AddDays(-30);
    public DateTime Hasta { get; set; } = DateTime.Today;

    public int TotalVentas { get; set; }
    public decimal SumaSubtotales { get; set; }
    public decimal SumaDescuentos { get; set; }
    public decimal SumaIva { get; set; }
    public decimal SumaPropinas { get; set; }
    public decimal SumaTotal { get; set; }
    public int VentasEfectivo { get; set; }
    public int VentasTarjeta { get; set; }

    public List<(string Nombre, int Cantidad)> TopProductos { get; set; } = [];

    public async Task OnGetAsync(DateTime? desde, DateTime? hasta)
    {
        Desde = desde ?? DateTime.Today.AddDays(-30);
        Hasta = hasta ?? DateTime.Today;

        var desdeUtc = Desde.ToUniversalTime();
        var hastaUtc = Hasta.AddDays(1).ToUniversalTime();

        var ventas = await _db.Ventas
            .Include(v => v.Pago)
            .Where(v => !v.Anulada && v.FechaCobro >= desdeUtc && v.FechaCobro < hastaUtc)
            .ToListAsync();

        TotalVentas = ventas.Count;
        SumaSubtotales = ventas.Sum(v => v.Subtotal);
        SumaDescuentos = ventas.Sum(v => v.MontoDescuento);
        SumaIva = ventas.Sum(v => v.Iva);
        SumaPropinas = ventas.Sum(v => v.Propina);
        SumaTotal = ventas.Sum(v => v.Total);
        VentasEfectivo = ventas.Count(v => v.Pago?.Metodo == Domain.Cobro.Enums.MetodoPago.Efectivo);
        VentasTarjeta = ventas.Count(v => v.Pago?.Metodo == Domain.Cobro.Enums.MetodoPago.Tarjeta);

        var ordenIds = ventas.Select(v => v.OrdenId).ToList();
        var lineas = await _db.LineasOrden
            .Where(l => ordenIds.Contains(l.OrdenId) && l.ProductoId.HasValue)
            .GroupBy(l => l.ProductoId!.Value)
            .Select(g => new { ProductoId = g.Key, Total = g.Sum(l => l.Cantidad) })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToListAsync();

        foreach (var l in lineas)
        {
            var nombre = (await _db.Productos.FindAsync(l.ProductoId))?.Nombre ?? "—";
            TopProductos.Add((nombre, l.Total));
        }
    }
}
