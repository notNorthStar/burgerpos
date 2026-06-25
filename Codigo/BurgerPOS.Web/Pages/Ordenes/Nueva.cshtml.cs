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

    [BindProperty]
    public Guid? MesaId { get; set; }

    public Orden? Orden { get; set; }
    public List<LineaVm> Lineas { get; set; } = new();
    public List<SelectListItem> ModalidadesSelect { get; set; } = new();
    public List<SelectListItem> ProductosSelect { get; set; } = new();
    public List<ComplementoVm> Complementos { get; set; } = new();
    public List<SelectListItem> MesasSelect { get; set; } = new();

    public async Task OnGetAsync(Guid? ordenId)
    {
        CargarModalidades();
        await CargarProductosAsync();
        await CargarMesasAsync();
        if (ordenId.HasValue)
        {
            Orden = await _context.Ordenes.Include(o => o.Lineas).Include(o => o.Mesa).FirstOrDefaultAsync(o => o.Id == ordenId);
            if (Orden is not null)
                Lineas = await BuildLineasAsync(Orden.Lineas);
        }
    }

    public async Task<IActionResult> OnPostCrearAsync()
    {
        var user = await _users.GetUserAsync(User);
        var mesaId = Modalidad == ModalidadServicio.Mesa ? MesaId : null;
        var orden = await _cobro.CrearOrdenAsync(Modalidad, user!.Id, mesaId);
        return RedirectToPage(new { ordenId = orden.Id });
    }

    public async Task<IActionResult> OnPostAgregarLineaAsync(Guid ordenId, Guid productoId, int cantidad,
        string? notaLibre, List<Guid>? complementoIds)
    {
        var producto = await _context.Productos.FindAsync(productoId);
        if (producto is not null)
            await _cobro.AgregarLineaAsync(ordenId, productoId, cantidad, producto.PrecioBase,
                string.IsNullOrWhiteSpace(notaLibre) ? null : notaLibre);

        if (complementoIds is not null)
        {
            foreach (var cid in complementoIds)
            {
                var comp = await _context.Productos.FindAsync(cid);
                if (comp is not null)
                    await _cobro.AgregarLineaAsync(ordenId, cid, 1, comp.PrecioBase);
            }
        }

        return RedirectToPage(new { ordenId });
    }

    public async Task<IActionResult> OnPostEliminarLineaAsync(Guid ordenId, Guid lineaId)
    {
        var linea = await _context.LineasOrden.FindAsync(lineaId);
        if (linea is not null) _context.LineasOrden.Remove(linea);
        await _context.SaveChangesAsync();

        var orden = await _context.Ordenes.Include(o => o.Lineas).FirstOrDefaultAsync(o => o.Id == ordenId);
        if (orden is not null) { orden.RecalcularSubtotal(); await _context.SaveChangesAsync(); }

        return RedirectToPage(new { ordenId });
    }

    public async Task<IActionResult> OnPostEnviarCocinaAsync(Guid ordenId)
    {
        var orden = await _context.Ordenes.Include(o => o.Lineas).FirstOrDefaultAsync(o => o.Id == ordenId);
        if (orden is not null && orden.Lineas.Any(l => l.NumeroEnvio == 0))
        {
            if (orden.Estado == Domain.Ordenes.Enums.EstadoOrden.Borrador)
                orden.EnviarACocina();
            else
                orden.ReenviarACocina();

            foreach (var linea in orden.Lineas.Where(l => l.NumeroEnvio == 0))
                linea.MarcarEnvio(orden.TotalEnvios);

            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { ordenId });
    }

    private async Task CargarMesasAsync()
    {
        var mesas = await _context.Mesas.OrderBy(m => m.Numero).ToListAsync();
        MesasSelect = mesas.Select(m => new SelectListItem($"Mesa {m.Numero}", m.Id.ToString())).ToList();
    }

    private void CargarModalidades()
    {
        ModalidadesSelect = Enum.GetValues<ModalidadServicio>()
            .Select(m => new SelectListItem(m.ToString(), m.ToString()))
            .ToList();
    }

    private async Task CargarProductosAsync()
    {
        var todos = await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Activo && p.Disponible)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        var complementos = todos.Where(p =>
            p.Categoria != null &&
            p.Categoria.Nombre.Contains("complemento", StringComparison.OrdinalIgnoreCase));

        var principales = todos.Except(complementos);

        ProductosSelect = principales
            .Select(p => new SelectListItem($"{p.Nombre} — ${p.PrecioBase:N2}", p.Id.ToString()))
            .ToList();

        Complementos = complementos
            .Select(p => new ComplementoVm(p.Id, p.Nombre, p.PrecioBase))
            .ToList();
    }

    private async Task<List<LineaVm>> BuildLineasAsync(IEnumerable<LineaOrden> lineas)
    {
        var result = new List<LineaVm>();
        foreach (var l in lineas)
        {
            var nombre = l.ProductoId.HasValue
                ? (await _context.Productos.FindAsync(l.ProductoId))?.Nombre ?? "—"
                : "Combo";
            result.Add(new LineaVm(l.Id, nombre, l.Cantidad, l.PrecioUnitario, l.NotaLibre));
        }
        return result;
    }

    public record LineaVm(Guid LineaId, string NombreProducto, int Cantidad, decimal PrecioUnitario, string? Nota);
    public record ComplementoVm(Guid Id, string Nombre, decimal Precio);
}
