using BurgerPOS.Domain.Inventario.Entities;
using BurgerPOS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BurgerPOS.Web.Pages.Inventario;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IInventarioService _inventario;

    public IndexModel(IInventarioService inventario) => _inventario = inventario;

    public List<Insumo> Insumos { get; set; } = new();

    public async Task OnGetAsync()
    {
        Insumos = await _inventario.ObtenerInsumosActivosAsync();
    }
}
