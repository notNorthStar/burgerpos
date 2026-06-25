using BurgerPOS.Domain.Administracion.Enums;
using BurgerPOS.Domain.Identidad.Enums;

namespace BurgerPOS.Application.Interfaces.Services;

public interface IBitacoraService
{
    Task RegistrarAsync(Guid usuarioId, TipoEvento tipo, string entidad,
        Guid? entidadId = null, string? valorAnterior = null, string? valorNuevo = null,
        RolOperativo? rolOperativo = null);
}
