using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Menu;

[Authorize]
public class CrearModel : PageModel
{
    private readonly ICatalogoService _catalogo;
    private readonly BurgerPosDbContext _context;

    public CrearModel(ICatalogoService catalogo, BurgerPosDbContext context)
    {
        _catalogo = catalogo;
        _context = context;
    }

    [BindProperty, Required]
    public string Nombre { get; set; } = string.Empty;

    [BindProperty]
    public string Descripcion { get; set; } = string.Empty;

    [BindProperty, Required, Range(0.01, 99999)]
    public decimal PrecioBase { get; set; }

    [BindProperty, Required]
    public Guid CategoriaId { get; set; }

    public List<SelectListItem> CategoriasSelect { get; set; } = new();

    public async Task OnGetAsync()
    {
        await CargarCategoriasAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarCategoriasAsync();
            return Page();
        }

        await _catalogo.CrearProductoAsync(Nombre, Descripcion, PrecioBase, CategoriaId);
        return RedirectToPage("/Menu/Index");
    }

    private async Task CargarCategoriasAsync()
    {
        var categorias = await _context.Categorias.OrderBy(c => c.OrdenVisual).ToListAsync();
        CategoriasSelect = categorias.Select(c => new SelectListItem(c.Nombre, c.Id.ToString())).ToList();
    }
}
