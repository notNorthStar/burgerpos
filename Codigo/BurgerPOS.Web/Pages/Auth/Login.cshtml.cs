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

    public LoginModel(SignInManager<ApplicationUser> signIn) => _signIn = signIn;

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
            return RedirectToPage("/Index");

        ModelState.AddModelError(string.Empty, "Correo o contrasena incorrectos.");
        return Page();
    }
}
