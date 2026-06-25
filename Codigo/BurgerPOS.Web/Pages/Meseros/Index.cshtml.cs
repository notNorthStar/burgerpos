using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Meseros;

[Authorize]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    public IndexModel(BurgerPosDbContext db) => _db = db;

    public List<Orden> Ordenes { get; set; } = [];

    public async Task OnGetAsync()
    {
        Ordenes = await _db.Ordenes
            .Include(o => o.Lineas)
            .Include(o => o.Mesa)
            .Where(o => o.Estado == EstadoOrden.Lista)
            .OrderBy(o => o.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostEntregarAsync(Guid ordenId)
    {
        var orden = await _db.Ordenes.FindAsync(ordenId);
        if (orden?.Estado == EstadoOrden.Lista)
        {
            orden.Entregar();
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
