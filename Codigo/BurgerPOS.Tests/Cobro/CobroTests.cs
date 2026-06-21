using BurgerPOS.Domain.Cobro.Enums;
using BurgerPOS.Domain.Ordenes.Enums;
using BurgerPOS.Infrastructure.Services;
using BurgerPOS.Tests.Fixtures;
using FluentAssertions;

namespace BurgerPOS.Tests.Cobro;

/// <summary>
/// CP-05 a CP-10: flujo de cobro, folio consecutivo, IVA, anulación (RF-05, RN-01..06).
/// </summary>
[Collection("Database")]
public class CobroTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public CobroTests(TestDatabaseFixture fixture) => _fixture = fixture;

    private async Task<(Guid productoId, Guid ordenId, Guid turnoId, Guid operadorId)> PrepararVentaAsync()
    {
        await using var ctx = _fixture.CreateContext();
        var catSvc = new CatalogoService(ctx);
        var cobroSvc = new CobroService(ctx);

        var cat = await catSvc.CrearCategoriaAsync($"Cat-{Guid.NewGuid():N}", 1);
        var producto = await catSvc.CrearProductoAsync($"Prod-{Guid.NewGuid():N}", "desc", 100m, cat.Id);
        var operadorId = Guid.NewGuid();
        var turno = await cobroSvc.AbrirTurnoAsync(operadorId, 500m);
        var orden = await cobroSvc.CrearOrdenAsync(ModalidadServicio.Mostrador, operadorId);
        await cobroSvc.AgregarLineaAsync(orden.Id, producto.Id, 1, 100m);

        return (producto.Id, orden.Id, turno.Id, operadorId);
    }

    [Fact]
    public async Task CobrarOrden_GeneraVentaConFolio()
    {
        var (_, ordenId, turnoId, operadorId) = await PrepararVentaAsync();

        await using var ctx = _fixture.CreateContext();
        var svc = new CobroService(ctx);
        var venta = await svc.CobrarAsync(ordenId, operadorId, turnoId, MetodoPago.Efectivo, montoRecibido: 200m);

        venta.Folio.Should().BeGreaterThan(0);
        venta.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CobrarOrden_CalculaIvaCorrectamente_RN01_RN06()
    {
        var (_, ordenId, turnoId, operadorId) = await PrepararVentaAsync();

        await using var ctx = _fixture.CreateContext();
        var svc = new CobroService(ctx);
        // subtotal=100, descuento=0, baseGravable=100, iva=16, propina=0 → total=116
        var venta = await svc.CobrarAsync(ordenId, operadorId, turnoId, MetodoPago.Efectivo, montoRecibido: 200m);

        venta.Subtotal.Should().Be(100m);
        venta.Iva.Should().Be(16m);
        venta.Total.Should().Be(116m);
    }

    [Fact]
    public async Task CobrarOrden_FoliosConsecutivos_RN02()
    {
        var (_, ordenId1, turnoId1, op1) = await PrepararVentaAsync();
        var (_, ordenId2, turnoId2, op2) = await PrepararVentaAsync();

        await using var ctx = _fixture.CreateContext();
        var svc = new CobroService(ctx);
        var v1 = await svc.CobrarAsync(ordenId1, op1, turnoId1, MetodoPago.Efectivo, montoRecibido: 200m);

        await using var ctx2 = _fixture.CreateContext();
        var svc2 = new CobroService(ctx2);
        var v2 = await svc2.CobrarAsync(ordenId2, op2, turnoId2, MetodoPago.Efectivo, montoRecibido: 200m);

        v2.Folio.Should().Be(v1.Folio + 1);
    }

    [Fact]
    public async Task CobrarOrden_MarcaOrdenComoCobrada()
    {
        var (_, ordenId, turnoId, operadorId) = await PrepararVentaAsync();

        await using var ctx = _fixture.CreateContext();
        var svc = new CobroService(ctx);
        await svc.CobrarAsync(ordenId, operadorId, turnoId, MetodoPago.Efectivo, montoRecibido: 200m);

        await using var verify = _fixture.CreateContext();
        var orden = await verify.Ordenes.FindAsync(ordenId);
        orden!.Estado.Should().Be(EstadoOrden.Cobrada);
    }

    [Fact]
    public async Task AnularVenta_SinRolAdmin_LanzaExcepcion_RN03()
    {
        var (_, ordenId, turnoId, operadorId) = await PrepararVentaAsync();

        await using var ctx = _fixture.CreateContext();
        var svc = new CobroService(ctx);
        var venta = await svc.CobrarAsync(ordenId, operadorId, turnoId, MetodoPago.Efectivo, montoRecibido: 200m);

        await using var ctx2 = _fixture.CreateContext();
        var svc2 = new CobroService(ctx2);
        var act = () => svc2.AnularVentaAsync(venta.Id, Guid.NewGuid(), esAdmin: false);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task AbrirTurno_GuardaFondoInicial()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new CobroService(ctx);
        var operadorId = Guid.NewGuid();

        var turno = await svc.AbrirTurnoAsync(operadorId, 1000m);

        turno.Id.Should().NotBeEmpty();

        await using var verify = _fixture.CreateContext();
        var guardado = await verify.TurnosCaja.FindAsync(turno.Id);
        guardado!.FondoInicial.Should().Be(1000m);
    }
}
