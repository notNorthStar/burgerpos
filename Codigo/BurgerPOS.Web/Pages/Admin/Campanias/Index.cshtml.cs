using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Domain.Catalogo.Enums;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Admin.Campanias;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    public IndexModel(BurgerPosDbContext db) => _db = db;

    public List<CampaniaDescuento> Campanias { get; set; } = [];

    [BindProperty, Required, MaxLength(100)] public string Nombre { get; set; } = string.Empty;
    [BindProperty] public TipoDescuento Tipo { get; set; } = TipoDescuento.Porcentaje;
    [BindProperty, Range(0.01, 9999)] public decimal Valor { get; set; }
    [BindProperty] public AlcanceDescuento Alcance { get; set; } = AlcanceDescuento.TodaVenta;
    [BindProperty] public DateTime FechaInicio { get; set; } = DateTime.Today;
    [BindProperty] public DateTime FechaFin { get; set; } = DateTime.Today.AddMonths(1);

    public async Task OnGetAsync()
    {
        Campanias = await _db.CampaniasDescuento.OrderByDescending(c => c.FechaInicio).ToListAsync();
    }

    public async Task<IActionResult> OnPostCrearAsync()
    {
        if (!ModelState.IsValid) { await OnGetAsync(); return Page(); }
        var c = CampaniaDescuento.Crear(Nombre, Tipo, Valor, Alcance,
            FechaInicio.ToUniversalTime(), FechaFin.ToUniversalTime());
        _db.CampaniasDescuento.Add(c);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostActivarAsync(Guid id)
    {
        var c = await _db.CampaniasDescuento.FindAsync(id);
        c?.Activar();
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPausarAsync(Guid id)
    {
        var c = await _db.CampaniasDescuento.FindAsync(id);
        c?.Pausar();
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostExpirarAsync(Guid id)
    {
        var c = await _db.CampaniasDescuento.FindAsync(id);
        c?.Expirar();
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}
