using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Menu;

[Authorize]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _context;

    public IndexModel(BurgerPosDbContext context) => _context = context;

    public List<Producto> Productos { get; set; } = new();
    public List<SelectListItem> Categorias { get; set; } = new();
    public Guid? CategoriaId { get; set; }

    public async Task OnGetAsync(Guid? categoriaId)
    {
        CategoriaId = categoriaId;

        Categorias = await _context.Categorias
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()))
            .ToListAsync();

        var query = _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Activo);

        if (categoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == categoriaId);

        Productos = await query.OrderBy(p => p.Nombre).ToListAsync();
    }
}
