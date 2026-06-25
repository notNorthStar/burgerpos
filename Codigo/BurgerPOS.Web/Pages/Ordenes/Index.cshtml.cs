using System.Security.Claims;
using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Administracion.Enums;
using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Ordenes;

[Authorize]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _context;
    private readonly IBitacoraService _bitacora;

    public IndexModel(BurgerPosDbContext context, IBitacoraService bitacora)
    {
        _context = context;
        _bitacora = bitacora;
    }

    public List<Orden> Ordenes { get; set; } = new();

    public async Task OnGetAsync()
    {
        Ordenes = await _context.Ordenes
            .Include(o => o.Mesa)
            .Where(o => o.Estado != EstadoOrden.Cobrada && o.Estado != EstadoOrden.Cancelada)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCancelarAsync(Guid ordenId)
    {
        var orden = await _context.Ordenes.FindAsync(ordenId);
        if (orden is not null)
        {
            orden.Cancelar();
            await _context.SaveChangesAsync();
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _bitacora.RegistrarAsync(userId, TipoEvento.CancelacionOrden, "Orden", ordenId);
        }
        return RedirectToPage();
    }
}
