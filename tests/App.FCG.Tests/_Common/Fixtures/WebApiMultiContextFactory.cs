using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Tests._Common.Fixtures;

public sealed class WebApiMultiContextFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private string _dbName = $"fcg-web-{Guid.NewGuid():N}";
    private readonly InMemoryDatabaseRoot _root = new();

    public void Reset() => _dbName = $"fcg-web-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Encontra todos os DbContextOptions<> registrados e remove
            var toRemove = services
                .Where(d => d.ServiceType.IsGenericType &&
                            d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))
                .ToList();

            foreach (var d in toRemove) services.Remove(d);

            // Re-registra todos os DbContexts concretos mapeados no container,
            // apontando para MESMO store InMemory
            var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && typeof(DbContext).IsAssignableFrom(t))
                .ToArray();

            foreach (var t in dbContextTypes)
            {
                var addDbCtx = typeof(EntityFrameworkServiceCollectionExtensions)
                    .GetMethods()
                    .Single(m => m.Name == nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext) &&
                                 m.IsGenericMethodDefinition &&
                                 m.GetParameters().Length == 2);

                var generic = addDbCtx.MakeGenericMethod(t);
                generic.Invoke(null, new object[]
                {
                    services,
                    (Action<DbContextOptionsBuilder>)(o =>
                        o.UseInMemoryDatabase(_dbName, _root).EnableSensitiveDataLogging())
                });
            }

            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();

            foreach (var t in dbContextTypes)
            {
                var ctx = (DbContext)scope.ServiceProvider.GetRequiredService(t);
                ctx.Database.EnsureCreated();
            }
        });
    }
}
