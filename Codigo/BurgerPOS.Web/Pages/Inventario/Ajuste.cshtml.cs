using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Administracion.Enums;
using BurgerPOS.Domain.Inventario.Entities;
using BurgerPOS.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Inventario;

[Authorize(Roles = "Admin")]
public class AjusteModel : PageModel
{
    private readonly IInventarioService _inventario;
    private readonly IBitacoraService _bitacora;
    private readonly UserManager<ApplicationUser> _users;

    public AjusteModel(IInventarioService inventario, IBitacoraService bitacora,
        UserManager<ApplicationUser> users)
    {
        _inventario = inventario;
        _bitacora = bitacora;
        _users = users;
    }

    public List<Insumo> Insumos { get; set; } = [];

    [BindProperty] public Guid InsumoId { get; set; }
    [BindProperty, Required, Range(0, 99999)] public decimal NuevaCantidad { get; set; }
    [BindProperty, Required, MinLength(5)] public string Motivo { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        Insumos = await _inventario.ObtenerInsumosActivosAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) { Insumos = await _inventario.ObtenerInsumosActivosAsync(); return Page(); }
        var user = await _users.GetUserAsync(User);
        var ajuste = await _inventario.AjustarSaldoAsync(InsumoId, NuevaCantidad, Motivo, user!.Id);
        await _bitacora.RegistrarAsync(user.Id, TipoEvento.AjusteSaldo, "AjusteSaldo", ajuste.Id,
            valorNuevo: $"{NuevaCantidad} — {Motivo}");
        return RedirectToPage("/Inventario/Index");
    }
}
