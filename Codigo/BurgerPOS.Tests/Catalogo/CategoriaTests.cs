using BurgerPOS.Infrastructure.Services;
using BurgerPOS.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BurgerPOS.Tests.Catalogo;

/// <summary>
/// CP-01: CRUD de categorías del menú (RF-02).
/// </summary>
[Collection("Database")]
public class CategoriaTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public CategoriaTests(TestDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task CrearCategoria_GuardaEnBase()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new CatalogoService(ctx);

        var categoria = await svc.CrearCategoriaAsync("Hamburguesas", 1);

        categoria.Id.Should().NotBeEmpty();

        await using var verify = _fixture.CreateContext();
        var guardada = await verify.Categorias.FindAsync(categoria.Id);
        guardada.Should().NotBeNull();
        guardada!.Nombre.Should().Be("Hamburguesas");
    }

    [Fact]
    public async Task ActualizarCategoria_CambiaValores()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new CatalogoService(ctx);
        var categoria = await svc.CrearCategoriaAsync("Bebidas", 2);

        await using var ctx2 = _fixture.CreateContext();
        var svc2 = new CatalogoService(ctx2);
        await svc2.ActualizarCategoriaAsync(categoria.Id, "Bebidas Frías", 3);

        await using var verify = _fixture.CreateContext();
        var actualizada = await verify.Categorias.FindAsync(categoria.Id);
        actualizada!.Nombre.Should().Be("Bebidas Frías");
        actualizada.OrdenVisual.Should().Be(3);
    }

    [Fact]
    public async Task EliminarCategoria_YaNoExisteEnBase()
    {
        await using var ctx = _fixture.CreateContext();
        var svc = new CatalogoService(ctx);
        var categoria = await svc.CrearCategoriaAsync("Temporales", 99);

        await using var ctx2 = _fixture.CreateContext();
        var svc2 = new CatalogoService(ctx2);
        await svc2.EliminarCategoriaAsync(categoria.Id);

        await using var verify = _fixture.CreateContext();
        var eliminada = await verify.Categorias.FindAsync(categoria.Id);
        eliminada.Should().BeNull();
    }
}
