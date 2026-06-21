using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Infrastructure.Identity;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Ordenes;

[Authorize]
public class NuevaModel : PageModel
{
    private readonly ICobroService _cobro;
    private readonly BurgerPosDbContext _context;
    private readonly UserManager<ApplicationUser> _users;

    public NuevaModel(ICobroService cobro, BurgerPosDbContext context, UserManager<ApplicationUser> users)
    {
        _cobro = cobro;
        _context = context;
        _users = users;
    }

    [BindProperty]
    public ModalidadServicio Modalidad { get; set; } = ModalidadServicio.Mostrador;

    public Orden? Orden { get; set; }
    public List<LineaVm> Lineas { get; set; } = new();
    public List<SelectListItem> ModalidadesSelect { get; set; } = new();
    public List<SelectListItem> ProductosSelect { get; set; } = new();

    public async Task OnGetAsync(Guid? ordenId)
    {
        CargarModalidades();
        await CargarProductosAsync();
        if (ordenId.HasValue)
        {
            Orden = await _context.Ordenes.Include(o => o.Lineas).FirstOrDefaultAsync(o => o.Id == ordenId);
            if (Orden is not null)
                Lineas = await BuildLineasAsync(Orden.Lineas);
        }
    }

    public async Task<IActionResult> OnPostCrearAsync()
    {
        var user = await _users.GetUserAsync(User);
        var orden = await _cobro.CrearOrdenAsync(Modalidad, user!.Id);
        return RedirectToPage(new { ordenId = orden.Id });
    }

    public async Task<IActionResult> OnPostAgregarLineaAsync(Guid ordenId, Guid productoId, int cantidad)
    {
        var producto = await _context.Productos.FindAsync(productoId);
        if (producto is not null)
            await _cobro.AgregarLineaAsync(ordenId, productoId, cantidad, producto.PrecioBase);
        return RedirectToPage(new { ordenId });
    }

    private void CargarModalidades()
    {
        ModalidadesSelect = Enum.GetValues<ModalidadServicio>()
            .Select(m => new SelectListItem(m.ToString(), m.ToString()))
            .ToList();
    }

    private async Task CargarProductosAsync()
    {
        var productos = await _context.Productos.Where(p => p.Activo && p.Disponible).OrderBy(p => p.Nombre).ToListAsync();
        ProductosSelect = productos.Select(p => new SelectListItem($"{p.Nombre} — ${p.PrecioBase:N2}", p.Id.ToString())).ToList();
    }

    private async Task<List<LineaVm>> BuildLineasAsync(IEnumerable<LineaOrden> lineas)
    {
        var result = new List<LineaVm>();
        foreach (var l in lineas)
        {
            var nombre = l.ProductoId.HasValue
                ? (await _context.Productos.FindAsync(l.ProductoId))?.Nombre ?? "—"
                : "Combo";
            result.Add(new LineaVm(nombre, l.Cantidad, l.PrecioUnitario));
        }
        return result;
    }

    public record LineaVm(string NombreProducto, int Cantidad, decimal PrecioUnitario);
}
