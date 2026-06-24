using BurgerPOS.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Web.Pages.Admin.Empleados;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _users;
    public IndexModel(UserManager<ApplicationUser> users) => _users = users;

    public List<(ApplicationUser User, IList<string> Roles)> Empleados { get; set; } = [];

    public async Task OnGetAsync()
    {
        var users = await _users.Users.OrderBy(u => u.NombreCompleto).ToListAsync();
        foreach (var u in users)
            Empleados.Add((u, await _users.GetRolesAsync(u)));
    }
}
