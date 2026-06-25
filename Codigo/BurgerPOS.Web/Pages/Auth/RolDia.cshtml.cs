using BurgerPOS.Domain.Identidad.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BurgerPOS.Web.Pages.Auth;

[Authorize]
public class RolDiaModel : PageModel
{
    [BindProperty]
    public RolOperativo RolOperativo { get; set; } = RolOperativo.EncargadoCobro;

    public void OnGet() { }

    public IActionResult OnPost()
    {
        HttpContext.Session.SetString("RolOperativo", RolOperativo.ToString());
        return RedirectToPage("/Index");
    }
}
