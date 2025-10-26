using FCG.Tests._Common.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace FCG.Tests._Common.Base;

public abstract class DbTestBase : IAsyncLifetime
{
    protected readonly EfInMemoryMultiContextFixture Db;

    protected DbTestBase(EfInMemoryMultiContextFixture db) => Db = db;

    public Task InitializeAsync() { Db.Reset(); return Task.CompletedTask; }
    public Task DisposeAsync() => Task.CompletedTask;

    protected TContext GetContext<TContext>() where TContext : DbContext => Db.Create<TContext>();
}
