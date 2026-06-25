using BurgerPOS.Application.Interfaces.Services;
using BurgerPOS.Domain.Administracion.Entities;
using BurgerPOS.Domain.Administracion.Enums;
using BurgerPOS.Domain.Identidad.Enums;
using BurgerPOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;

namespace BurgerPOS.Infrastructure.Services;

public class BitacoraService : IBitacoraService
{
    private readonly BurgerPosDbContext _context;
    private readonly IHttpContextAccessor _http;

    public BitacoraService(BurgerPosDbContext context, IHttpContextAccessor http)
    {
        _context = context;
        _http = http;
    }

    public async Task RegistrarAsync(Guid usuarioId, TipoEvento tipo, string entidad,
        Guid? entidadId = null, string? valorAnterior = null, string? valorNuevo = null,
        RolOperativo? rolOperativo = null)
    {
        var ip = _http.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "local";
        var evento = BitacoraEvento.Registrar(usuarioId, tipo, entidad, ip,
            entidadId, valorAnterior, valorNuevo, rolOperativo);
        _context.BitacoraEventos.Add(evento);
        await _context.SaveChangesAsync();
    }
}
