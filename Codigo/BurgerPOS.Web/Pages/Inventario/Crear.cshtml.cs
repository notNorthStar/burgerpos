using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Inventario.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Inventario;

[Authorize]
public class CrearModel : PageModel
{
    private readonly IInventarioService _inventario;

    public CrearModel(IInventarioService inventario) => _inventario = inventario;

    [BindProperty, Required]
    public string Nombre { get; set; } = string.Empty;

    [BindProperty]
    public UnidadMedida Unidad { get; set; } = UnidadMedida.Piezas;

    [BindProperty, Required, Range(0, 99999)]
    public decimal SaldoInicial { get; set; }

    [BindProperty, Required, Range(0, 99999)]
    public decimal NivelAlerta { get; set; }

    public List<SelectListItem> UnidadesSelect { get; set; } = Enum.GetValues<UnidadMedida>()
        .Select(u => new SelectListItem(u.ToString(), u.ToString()))
        .ToList();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _inventario.CrearInsumoAsync(Nombre, Unidad, SaldoInicial, NivelAlerta);
        return RedirectToPage("/Inventario/Index");
    }
}
