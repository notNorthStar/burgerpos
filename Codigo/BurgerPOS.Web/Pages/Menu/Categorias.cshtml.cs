using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Catalogo.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Menu;

[Authorize]
public class CategoriasModel : PageModel
{
    private readonly ICatalogoService _catalogo;

    public CategoriasModel(ICatalogoService catalogo) => _catalogo = catalogo;

    [BindProperty, Required]
    public string Nombre { get; set; } = string.Empty;

    public List<Categoria> Categorias { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categorias = await _catalogo.ObtenerCategoriasAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Categorias = await _catalogo.ObtenerCategoriasAsync();
            return Page();
        }
        await _catalogo.CrearCategoriaAsync(Nombre);
        return RedirectToPage();
    }
}
