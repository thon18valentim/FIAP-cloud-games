using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FCG.Tests._Common.Fixtures;

/// <summary>
/// Fixture que fornece um InMemory compartilhado entre N DbContexts.
/// Cresce por registro de fábrica. Garante isolamento por classe de teste.
/// </summary>
public sealed class EfInMemoryMultiContextFixture : IAsyncLifetime
{
    private readonly string _dbName = $"fcg-tests-{Guid.NewGuid():N}";
    private readonly InMemoryDatabaseRoot _root = new();

    // Registry de fábricas por tipo de DbContext
    private readonly ConcurrentDictionary<Type, Func<DbContextOptions, DbContext>> _factories = new();

    public EfInMemoryMultiContextFixture Register<TContext>(Func<DbContextOptions<TContext>, TContext> factory)
        where TContext : DbContext
    {
        _factories[typeof(TContext)] = (opt) => factory((DbContextOptions<TContext>)opt);
        return this;
    }

    public TContext Create<TContext>() where TContext : DbContext
    {
        if (!_factories.TryGetValue(typeof(TContext), out var factory))
            throw new InvalidOperationException($"Nenhuma fábrica registrada para {typeof(TContext).Name}.");

        var options = new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(_dbName, _root)
            .EnableSensitiveDataLogging()
            .Options;

        return (TContext)factory(options);
    }

    /// <summary>Reseta o store compartilhado (limpeza entre testes).</summary>
    public void Reset()
    {
        foreach (var kvp in _factories)
        {
            using var ctx = CreateInternal(kvp.Key);
            ctx.Database.EnsureDeleted();
        }

        foreach (var kvp in _factories)
        {
            using var ctx = CreateInternal(kvp.Key);
            ctx.Database.EnsureCreated();
        }
    }

    public Task InitializeAsync()
    {
        foreach (var kvp in _factories)
        {
            using var ctx = CreateInternal(kvp.Key);
            ctx.Database.EnsureCreated();
        }
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private DbContext CreateInternal(Type t)
    {
        if (!_factories.TryGetValue(t, out var factory))
            throw new InvalidOperationException($"Nenhuma fábrica registrada para {t.Name}.");

        var dbCtxOptionsType = typeof(DbContextOptions<>).MakeGenericType(t);
        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(t);
        var builder = Activator.CreateInstance(optionsBuilderType)!;

        var useInMemory = optionsBuilderType.GetMethod("UseInMemoryDatabase", new[] { typeof(string), typeof(InMemoryDatabaseRoot) })!;
        useInMemory.Invoke(builder, new object[] { _dbName, _root });

        var enableSDL = optionsBuilderType.GetMethod("EnableSensitiveDataLogging", Type.EmptyTypes)!;
        enableSDL.Invoke(builder, Array.Empty<object>());

        var options = optionsBuilderType.GetProperty("Options")!.GetValue(builder)!;
        return factory((DbContextOptions)options);
    }
}
