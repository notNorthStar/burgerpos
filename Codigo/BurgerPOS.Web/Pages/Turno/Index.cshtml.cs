using System.Security.Claims;
using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Turno.Entities;
using BurgerPOS.Domain.Turno.Enums;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Turno;

[Authorize]
public class IndexModel : PageModel
{
    private readonly BurgerPosDbContext _db;
    private readonly ICobroService _cobro;

    public IndexModel(BurgerPosDbContext db, ICobroService cobro)
    {
        _db = db;
        _cobro = cobro;
    }

    public TurnoCaja? TurnoActual { get; set; }
    public string? Mensaje { get; set; }

    [BindProperty, Range(0, 999999)] public decimal FondoInicial { get; set; }
    [BindProperty, Range(0, 999999)] public decimal EfectivoContado { get; set; }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task OnGetAsync() =>
        TurnoActual = await _db.TurnosCaja
            .Where(t => t.OperadorAperturaId == UserId && t.Estado != EstadoTurno.Cerrado)
            .OrderByDescending(t => t.FechaApertura)
            .FirstOrDefaultAsync();

    public async Task<IActionResult> OnPostAbrirAsync()
    {
        await _cobro.AbrirTurnoAsync(UserId, FondoInicial);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostArquearAsync(Guid turnoId)
    {
        var turno = await _db.TurnosCaja.FindAsync(turnoId);
        if (turno is null) return RedirectToPage();
        turno.Arquear(EfectivoContado, UserId);
        turno.Cerrar();
        await _db.SaveChangesAsync();
        Mensaje = $"Turno cerrado. Diferencia: {turno.Diferencia:C}";
        TurnoActual = null;
        return Page();
    }
}
