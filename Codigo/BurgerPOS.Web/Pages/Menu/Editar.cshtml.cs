using BurgerPOS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Menu;

[Authorize]
public class EditarModel : PageModel
{
    private readonly ICatalogoService _catalogo;

    public EditarModel(ICatalogoService catalogo) => _catalogo = catalogo;

    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty, Required]
    public string Nombre { get; set; } = string.Empty;

    [BindProperty, Required, Range(0.01, 99999)]
    public decimal PrecioBase { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var producto = await _catalogo.ObtenerProductoAsync(id);
        if (producto is null) return NotFound();
        Id = producto.Id;
        Nombre = producto.Nombre;
        PrecioBase = producto.PrecioBase;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _catalogo.ActualizarPrecioAsync(Id, PrecioBase);
        return RedirectToPage("/Menu/Index");
    }
}
