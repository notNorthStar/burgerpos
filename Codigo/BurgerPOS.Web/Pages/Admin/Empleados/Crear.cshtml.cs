using BurgerPOS.Domain.Identidad.Enums;
using BurgerPOS.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Admin.Empleados;

[Authorize(Roles = "Admin")]
public class CrearModel : PageModel
{
    private readonly UserManager<ApplicationUser> _users;
    public CrearModel(UserManager<ApplicationUser> users) => _users = users;

    [BindProperty, Required] public string NombreCompleto { get; set; } = string.Empty;
    [BindProperty, Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [BindProperty, Required] public string Password { get; set; } = string.Empty;
    [BindProperty, Required] public string RolSeleccionado { get; set; } = "Cajero";

    public List<SelectListItem> Roles { get; set; } =
    [
        new("Cajero", "Cajero"),
        new("Mesero", "Mesero"),
        new("Cocinero", "Cocinero"),
        new("Admin", "Admin"),
    ];

    public string? Error { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = new ApplicationUser
        {
            UserName = Email,
            Email = Email,
            NombreCompleto = NombreCompleto,
            Rol = RolSeleccionado == "Admin" ? Rol.Administrador : Rol.Operador
        };

        var result = await _users.CreateAsync(user, Password);
        if (!result.Succeeded)
        {
            Error = string.Join(", ", result.Errors.Select(e => e.Description));
            return Page();
        }

        await _users.AddToRoleAsync(user, RolSeleccionado);
        return RedirectToPage("/Admin/Empleados/Index");
    }
}
