using BurgerPOS.Infrastructure.Services;
using BurgerPOS.Tests.Fixtures;
using FluentAssertions;

namespace BurgerPOS.Tests.Catalogo;

/// <summary>
/// CP-02: Crear producto y actualizar precio (RF-02, RN-04).
/// </summary>
[Collection("Database")]
public class ProductoTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public ProductoTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CrearProducto_GuardaPrecioCorrectamente()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new CatalogoService(ctx);
        var cat = await svc.CrearCategoriaAsync("Hamburguesas CP02", 1);

        var producto = await svc.CrearProductoAsync("Doble Smash", "2 carnes", 89.00m, cat.Id);

        producto.Id.Should().NotBeEmpty();

        await using var verify = _fixture.CreateContext();
        var guardado = await verify.Productos.FindAsync(producto.Id);
        guardado.Should().NotBeNull();
        guardado!.PrecioBase.Should().Be(89.00m);
        guardado.CategoriaId.Should().Be(cat.Id);
    }

    [Fact]
    public async Task ActualizarPrecio_ReflejaHistoricoEnNuevasVentas()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new CatalogoService(ctx);
        var cat = await svc.CrearCategoriaAsync("Cat Precio", 5);
        var producto = await svc.CrearProductoAsync("Triple", "3 carnes", 110.00m, cat.Id);

        await using var ctx2 = _fixture.CreateContext();
        var svc2 = new CatalogoService(ctx2);
        await svc2.ActualizarPrecioAsync(producto.Id, 125.00m);

        await using var verify = _fixture.CreateContext();
        var actualizado = await verify.Productos.FindAsync(producto.Id);
        actualizado!.PrecioBase.Should().Be(125.00m);
    }
}
