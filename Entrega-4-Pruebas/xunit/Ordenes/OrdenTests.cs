using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Infrastructure.Services;
using BurgerPOS.Tests.Fixtures;
using FluentAssertions;

namespace BurgerPOS.Tests.Ordenes;

/// <summary>
/// CP-04: Crear orden y agregar líneas (RF-04).
/// </summary>
[Collection("Database")]
public class OrdenTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public OrdenTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CrearOrden_EstadoInicialEsAbierta()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new CobroService(ctx);
        var operadorId = Guid.NewGuid();

        var orden = await svc.CrearOrdenAsync(ModalidadServicio.Mostrador, operadorId);

        orden.Id.Should().NotBeEmpty();
        orden.Estado.Should().Be(EstadoOrden.Borrador);

        await using var verify = _fixture.CreateContext();
        var guardada = await verify.Ordenes.FindAsync(orden.Id);
        guardada.Should().NotBeNull();
        guardada!.Estado.Should().Be(EstadoOrden.Borrador);
    }

    [Fact]
    public async Task AgregarLinea_ActualizaSubtotalDeLaOrden()
    {
        await using var ctx = _fixture.CreateContext();
        var catSvc = new CatalogoService(ctx);
        var cobroSvc = new CobroService(ctx);
        var cat = await catSvc.CrearCategoriaAsync("Cat Orden", 1);
        var producto = await catSvc.CrearProductoAsync("Classic Burger", "La original", 75m, cat.Id);
        var operadorId = Guid.NewGuid();
        var orden = await cobroSvc.CrearOrdenAsync(ModalidadServicio.Mostrador, operadorId);

        await using var ctx2 = _fixture.CreateContext();
        var cobroSvc2 = new CobroService(ctx2);
        await cobroSvc2.AgregarLineaAsync(orden.Id, producto.Id, 2, 75m);

        await using var verify = _fixture.CreateContext();
        var actualizada = await verify.Ordenes.FindAsync(orden.Id);
        actualizada!.Subtotal.Should().Be(150m);
    }
}
