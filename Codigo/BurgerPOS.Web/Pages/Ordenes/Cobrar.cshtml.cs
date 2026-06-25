using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Catalogo.Entities;
using BurgerPOS.Domain.Cobro.Enums;
using BurgerPOS.Domain.Ordenes.Entities;
using BurgerPOS.Domain.Turno.Enums;
using BurgerPOS.Infrastructure.Identity;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Ordenes;

[Authorize]
public class CobrarModel : PageModel
{
    private readonly ICobroService _cobro;
    private readonly IBitacoraService _bitacora;
    private readonly BurgerPosDbContext _context;
    private readonly UserManager<ApplicationUser> _users;

    public CobrarModel(ICobroService cobro, IBitacoraService bitacora,
        BurgerPosDbContext context, UserManager<ApplicationUser> users)
    {
        _cobro = cobro;
        _bitacora = bitacora;
        _context = context;
        _users = users;
    }

    public Orden? Orden { get; set; }
    public List<CampaniaDescuento> CampaniasVigentes { get; set; } = [];
    [BindProperty] public Guid? CampaniaId { get; set; }

    public async Task OnGetAsync(Guid ordenId)
    {
        Orden = await _context.Ordenes.Include(o => o.Lineas).FirstOrDefaultAsync(o => o.Id == ordenId);
        await CargarCampaniasAsync();
    }

    public async Task<IActionResult> OnPostAsync(Guid ordenId, string metodoPago, decimal montoRecibido, decimal propina)
    {
        var user = await _users.GetUserAsync(User);
        var userId = user!.Id;

        var turno = await _context.TurnosCaja
            .Where(t => t.OperadorAperturaId == userId &&
                        (t.Estado == EstadoTurno.EnOperacion || t.Estado == EstadoTurno.Abierto))
            .OrderByDescending(t => t.FechaApertura)
            .FirstOrDefaultAsync();

        Guid turnoId;
        if (turno is null)
        {
            var nuevo = await _cobro.AbrirTurnoAsync(userId, 0);
            turnoId = nuevo.Id;
        }
        else
        {
            turnoId = turno.Id;
        }

        var metodo = metodoPago == "Tarjeta" ? MetodoPago.Tarjeta : MetodoPago.Efectivo;
        var venta = await _cobro.CobrarAsync(ordenId, userId, turnoId, metodo, propina, montoRecibido,
            campaniaDescuentoId: CampaniaId);

        await _bitacora.RegistrarAsync(userId, Domain.Administracion.Enums.TipoEvento.Venta,
            "Venta", venta.Id, valorNuevo: $"${venta.Total:N2}");

        return RedirectToPage("/Ventas/Ticket", new { ventaId = venta.Id });
    }

    private async Task CargarCampaniasAsync()
    {
        var ahora = DateTime.UtcNow;
        var todas = await _context.CampaniasDescuento.ToListAsync();
        CampaniasVigentes = todas.Where(c => c.EstaVigente(ahora)).ToList();
    }
}
