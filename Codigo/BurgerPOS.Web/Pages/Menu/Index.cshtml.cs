using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Menu;

[Authorize]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _context;

    public IndexModel(BurgerPosDbContext context) => _context = context;

    public List<Producto> Productos { get; set; } = new();

    public async Task OnGetAsync()
    {
        Productos = await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }
}
