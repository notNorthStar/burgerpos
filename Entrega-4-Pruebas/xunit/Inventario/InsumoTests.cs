using BurgerPOS.Domain.Inventario.Enums;
using BurgerPOS.Infrastructure.Services;
using BurgerPOS.Tests.Fixtures;
using FluentAssertions;

namespace BurgerPOS.Tests.Inventario;

/// <summary>
/// CP-03: CRUD insumos y movimientos de inventario (RF-07, RN-08).
/// </summary>
[Collection("Database")]
public class InsumoTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public InsumoTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CrearInsumo_GuardaSaldoInicial()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new InventarioService(ctx);

        var insumo = await svc.CrearInsumoAsync("Carne 80/20", UnidadMedida.Kilogramos, 10m, 2m);

        insumo.Id.Should().NotBeEmpty();

        await using var verify = _fixture.CreateContext();
        var guardado = await verify.Insumos.FindAsync(insumo.Id);
        guardado!.SaldoActual.Should().Be(10m);
        guardado.NivelAlerta.Should().Be(2m);
    }

    [Fact]
    public async Task RegistrarEntrada_IncrementaSaldo()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new InventarioService(ctx);
        var insumo = await svc.CrearInsumoAsync("Pan Brioche", UnidadMedida.Piezas, 50m, 10m);
        var usuarioId = Guid.NewGuid();

        await using var ctx2 = _fixture.CreateContext();
        var svc2 = new InventarioService(ctx2);
        await svc2.RegistrarEntradaAsync(insumo.Id, 20m, usuarioId);

        await using var verify = _fixture.CreateContext();
        var actualizado = await verify.Insumos.FindAsync(insumo.Id);
        actualizado!.SaldoActual.Should().Be(70m);
    }

    [Fact]
    public async Task RegistrarMerma_DecrementaSaldo()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new InventarioService(ctx);
        var insumo = await svc.CrearInsumoAsync("Queso Americano", UnidadMedida.Kilogramos, 5m, 1m);
        var usuarioId = Guid.NewGuid();

        await using var ctx2 = _fixture.CreateContext();
        var svc2 = new InventarioService(ctx2);
        await svc2.RegistrarMermaAsync(insumo.Id, 1.5m, MotivoMerma.Caducidad, usuarioId);

        await using var verify = _fixture.CreateContext();
        var actualizado = await verify.Insumos.FindAsync(insumo.Id);
        actualizado!.SaldoActual.Should().Be(3.5m);
    }

    [Fact]
    public async Task RegistrarMerma_StockInsuficiente_LanzaExcepcion()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new InventarioService(ctx);
        var insumo = await svc.CrearInsumoAsync("Pepinillos", UnidadMedida.Kilogramos, 0.5m, 0.2m);
        var usuarioId = Guid.NewGuid();

        await using var ctx2 = _fixture.CreateContext();
        var svc2 = new InventarioService(ctx2);
        var act = () => svc2.RegistrarMermaAsync(insumo.Id, 2m, MotivoMerma.Otro, usuarioId);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
