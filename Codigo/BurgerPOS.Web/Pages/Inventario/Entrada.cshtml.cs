using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Infrastructure.Identity;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Inventario;

[Authorize]
public class EntradaModel : PageModel
{
    private readonly IInventarioService _inventario;
    private readonly BurgerPosDbContext _context;
    private readonly UserManager<ApplicationUser> _users;

    public EntradaModel(IInventarioService inventario, BurgerPosDbContext context, UserManager<ApplicationUser> users)
    {
        _inventario = inventario;
        _context = context;
        _users = users;
    }

    [BindProperty, Required]
    public Guid InsumoId { get; set; }

    [BindProperty, Required, Range(0.01, 99999)]
    public decimal Cantidad { get; set; }

    [BindProperty]
    public string? Proveedor { get; set; }

    public List<SelectListItem> InsumosSelect { get; set; } = new();

    public async Task OnGetAsync()
    {
        await CargarInsumosAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarInsumosAsync();
            return Page();
        }

        var user = await _users.GetUserAsync(User);
        await _inventario.RegistrarEntradaAsync(InsumoId, Cantidad, user!.Id, Proveedor);
        return RedirectToPage("/Inventario/Index");
    }

    private async Task CargarInsumosAsync()
    {
        var insumos = await _context.Insumos.Where(i => i.Activo).OrderBy(i => i.Nombre).ToListAsync();
        InsumosSelect = insumos.Select(i => new SelectListItem($"{i.Nombre} (saldo: {i.SaldoActual:N2})", i.Id.ToString())).ToList();
    }
}
