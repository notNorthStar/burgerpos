using System.Security.Claims;
using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Cobro.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Admin.Ventas;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    private readonly ICobroService _cobro;

    public IndexModel(BurgerPosDbContext db, ICobroService cobro)
    {
        _db = db;
        _cobro = cobro;
    }

    public List<Venta> Ventas { get; set; } = [];
    public string? Mensaje { get; set; }

    public async Task OnGetAsync() =>
        Ventas = await _db.Ventas
            .OrderByDescending(v => v.FechaCobro)
            .Take(100)
            .ToListAsync();

    public async Task<IActionResult> OnPostAnularAsync(Guid ventaId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _cobro.AnularVentaAsync(ventaId, userId, esAdmin: true);
        return RedirectToPage();
    }
}
