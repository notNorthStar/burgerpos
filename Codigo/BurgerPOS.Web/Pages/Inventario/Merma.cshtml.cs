using System.Security.Claims;
using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Inventario.Entities;
using BurgerPOS.Domain.Inventario.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Inventario;

[Authorize]
public class MermaModel : PageModel
{
    private readonly IInventarioService _inventario;
    public MermaModel(IInventarioService inventario) => _inventario = inventario;

    [BindProperty] public Guid InsumoId { get; set; }

    [BindProperty, Required, Range(0.01, 99999)]
    public decimal Cantidad { get; set; }

    [BindProperty] public MotivoMerma Motivo { get; set; } = MotivoMerma.Otro;

    [BindProperty] public string? Descripcion { get; set; }

    public List<Insumo> Insumos { get; set; } = [];

    public List<SelectListItem> MotivosSelect { get; set; } = Enum.GetValues<MotivoMerma>()
        .Select(m => new SelectListItem(m.ToString(), m.ToString()))
        .ToList();

    public async Task OnGetAsync() =>
        Insumos = await _inventario.ObtenerInsumosActivosAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Insumos = await _inventario.ObtenerInsumosActivosAsync();
            return Page();
        }
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _inventario.RegistrarMermaAsync(InsumoId, Cantidad, Motivo, userId, Descripcion);
        return RedirectToPage("/Inventario/Index");
    }
}
