using BurgerPOS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace BurgerPOS.Tests.Fixtures;

public class TestDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("burgerpos_test")
        .WithUsername("burgerpos")
        .WithPassword("test")
        .Build();

    public BurgerPosDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BurgerPosDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options;
        return new BurgerPosDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var ctx = CreateContext();
        await ctx.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();
}
