using BurgerPOS.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BurgerPOS.Web.Pages.Auth;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly UserManager<ApplicationUser> _users;

    public LoginModel(SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> users)
    {
        _signIn = signIn;
        _users = users;
    }

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Password { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _signIn.PasswordSignInAsync(Email, Password, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            var user = await _users.FindByEmailAsync(Email);
            if (user is not null && await _users.IsInRoleAsync(user, "Admin"))
                return RedirectToPage("/Index");
            return RedirectToPage("/Auth/RolDia");
        }

        ModelState.AddModelError(string.Empty, "Correo o contrasena incorrectos.");
        return Page();
    }
}
