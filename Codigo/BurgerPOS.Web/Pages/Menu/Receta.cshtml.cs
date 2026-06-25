using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Domain.Inventario.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Menu;

[Authorize]
public class RecetaModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    public RecetaModel(BurgerPosDbContext db) => _db = db;

    public Producto? Producto { get; set; }
    public Receta? Receta { get; set; }
    public List<Insumo> InsumosDisponibles { get; set; } = [];

    [BindProperty] public Guid InsumoId { get; set; }
    [BindProperty, Required, Range(0.001, 9999)] public decimal Cantidad { get; set; }

    private async Task CargarAsync(Guid productoId)
    {
        Producto = await _db.Productos.FindAsync(productoId);
        Receta = await _db.Recetas
            .Include(r => r.Lineas)
            .FirstOrDefaultAsync(r => r.ProductoId == productoId);
        InsumosDisponibles = await _db.Insumos.Where(i => i.Activo).OrderBy(i => i.Nombre).ToListAsync();
    }

    public async Task OnGetAsync(Guid id) => await CargarAsync(id);

    public async Task<IActionResult> OnPostAgregarAsync(Guid id)
    {
        if (!ModelState.IsValid) { await CargarAsync(id); return Page(); }

        var receta = await _db.Recetas
            .Include(r => r.Lineas)
            .FirstOrDefaultAsync(r => r.ProductoId == id);

        if (receta is null)
        {
            receta = Receta.Crear(id);
            _db.Recetas.Add(receta);
        }

        receta.AgregarLinea(InsumoId, Cantidad);
        await _db.SaveChangesAsync();
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEliminarAsync(Guid id, Guid insumoId)
    {
        var linea = await _db.LineasReceta.FindAsync(
            (await _db.Recetas.FirstOrDefaultAsync(r => r.ProductoId == id))!.Id,
            insumoId);
        if (linea is not null) _db.LineasReceta.Remove(linea);
        await _db.SaveChangesAsync();
        return RedirectToPage(new { id });
    }
}
