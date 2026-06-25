using BurgerPOS.Domain.Administracion.Entities;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Admin.Bitacora;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    public IndexModel(BurgerPosDbContext db) => _db = db;

    public List<BitacoraEvento> Eventos { get; set; } = [];

    public async Task OnGetAsync()
    {
        Eventos = await _db.BitacoraEventos
            .OrderByDescending(e => e.Timestamp)
            .Take(200)
            .ToListAsync();
    }
}
