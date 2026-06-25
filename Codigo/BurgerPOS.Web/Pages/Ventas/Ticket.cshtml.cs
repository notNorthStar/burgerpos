using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Domain.Cobro.Entities;
using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Ventas;

[Authorize]
public class TicketModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    public TicketModel(BurgerPosDbContext db) => _db = db;

    public Venta? Venta { get; set; }
    public Pago? Pago { get; set; }
    public Orden? Orden { get; set; }
    public List<LineaVm> Lineas { get; set; } = [];
    public DatosEstablecimiento? Establecimiento { get; set; }
    public string? NombreCampania { get; set; }

    public async Task OnGetAsync(Guid ventaId)
    {
        Venta = await _db.Ventas
            .Include(v => v.Pago)
            .Include(v => v.DescuentoAplicado)
            .FirstOrDefaultAsync(v => v.Id == ventaId);
        if (Venta is null) return;

        Pago = Venta.Pago;

        if (Venta.DescuentoAplicado is not null)
        {
            var camp = await _db.CampaniasDescuento.FindAsync(Venta.DescuentoAplicado.CampaniaId);
            NombreCampania = camp?.Nombre;
        }

        Orden = await _db.Ordenes
            .Include(o => o.Mesa)
            .Include(o => o.Lineas)
            .FirstOrDefaultAsync(o => o.Id == Venta.OrdenId);

        if (Orden is not null)
        {
            foreach (var l in Orden.Lineas)
            {
                string nombre = "—";
                if (l.ProductoId.HasValue)
                    nombre = (await _db.Productos.FindAsync(l.ProductoId.Value))?.Nombre ?? "—";
                else if (l.ComboId.HasValue)
                    nombre = (await _db.Combos.FindAsync(l.ComboId.Value))?.Nombre ?? "Combo";
                Lineas.Add(new LineaVm(nombre, l.Cantidad, l.PrecioUnitario, l.NotaLibre));
            }
        }

        Establecimiento = await _db.DatosEstablecimiento.FirstOrDefaultAsync();
    }

    public record LineaVm(string Nombre, int Cantidad, decimal PrecioUnitario, string? Nota);
}
