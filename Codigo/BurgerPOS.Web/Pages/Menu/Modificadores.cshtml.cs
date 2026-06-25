using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Menu;

[Authorize]
public class ModificadoresModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    public ModificadoresModel(BurgerPosDbContext db) => _db = db;

    public Producto? Producto { get; set; }
    public List<Modificador> Lista { get; set; } = [];

    [BindProperty, Required, MaxLength(100)] public string Nombre { get; set; } = string.Empty;
    [BindProperty] public decimal DeltaPrecio { get; set; }

    private async Task CargarAsync(Guid id)
    {
        Producto = await _db.Productos.Include(p => p.Modificadores).FirstOrDefaultAsync(p => p.Id == id);
        Lista = Producto?.Modificadores.ToList() ?? [];
    }

    public async Task OnGetAsync(Guid id) => await CargarAsync(id);

    public async Task<IActionResult> OnPostAgregarAsync(Guid id)
    {
        if (!ModelState.IsValid) { await CargarAsync(id); return Page(); }
        _db.Modificadores.Add(Modificador.Crear(id, Nombre, DeltaPrecio));
        await _db.SaveChangesAsync();
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEliminarAsync(Guid id, Guid modificadorId)
    {
        var m = await _db.Modificadores.FindAsync(modificadorId);
        if (m is not null) _db.Modificadores.Remove(m);
        await _db.SaveChangesAsync();
        return RedirectToPage(new { id });
    }
}
