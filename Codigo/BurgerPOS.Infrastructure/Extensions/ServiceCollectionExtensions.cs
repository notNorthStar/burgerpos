using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Infrastructure.Identity;
using BurgerPOS.Infrastructure.Services;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BurgerPOS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<BurgerPosDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<BurgerPosDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICatalogoService, CatalogoService>();
        services.AddScoped<IInventarioService, InventarioService>();
        services.AddScoped<ICobroService, CobroService>();
        services.AddScoped<IBitacoraService, BitacoraService>();
        return services;
    }
}
