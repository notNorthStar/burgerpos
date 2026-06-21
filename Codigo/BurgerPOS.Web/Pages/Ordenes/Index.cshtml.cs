using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Ordenes;

[Authorize]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _context;

    public IndexModel(BurgerPosDbContext context) => _context = context;

    public List<Orden> Ordenes { get; set; } = new();

    public async Task OnGetAsync()
    {
        Ordenes = await _context.Ordenes
            .Where(o => o.Estado != EstadoOrden.Cobrada && o.Estado != EstadoOrden.Cancelada)
            .OrderByDescending(o => o.FechaCreacion)
            .ToListAsync();
    }
}
