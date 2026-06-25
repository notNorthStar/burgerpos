using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Admin.Configuracion;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    public IndexModel(BurgerPosDbContext db) => _db = db;

    [BindProperty, Required, MaxLength(150)] public string NombreComercial { get; set; } = string.Empty;
    [BindProperty, Required, MaxLength(250)] public string Direccion { get; set; } = string.Empty;
    [BindProperty, Required, MaxLength(30)] public string Telefono { get; set; } = string.Empty;
    [BindProperty, MaxLength(13)] public string? Rfc { get; set; }
    [BindProperty, MaxLength(200)] public string? LeyendaTicket { get; set; }

    public async Task OnGetAsync()
    {
        var d = await _db.DatosEstablecimiento.FirstOrDefaultAsync();
        if (d is null) return;
        NombreComercial = d.NombreComercial;
        Direccion = d.Direccion;
        Telefono = d.Telefono;
        Rfc = d.Rfc;
        LeyendaTicket = d.LeyendaTicket;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var d = await _db.DatosEstablecimiento.FirstOrDefaultAsync();
        if (d is null)
        {
            d = DatosEstablecimiento.Crear(NombreComercial, Direccion, Telefono, Rfc, LeyendaTicket);
            _db.DatosEstablecimiento.Add(d);
        }
        else
        {
            d.Actualizar(NombreComercial, Direccion, Telefono, Rfc, LeyendaTicket);
        }
        await _db.SaveChangesAsync();
        TempData["ok"] = "Datos guardados.";
        return RedirectToPage();
    }
}
