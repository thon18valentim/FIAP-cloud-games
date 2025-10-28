using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using FCG.Core.Data;
using FCG.Games.Models;

namespace FCG.Games.Data;

public sealed class GameContext : DbContext, IUnitOfWork
{
    public DbSet<Game> Games { get; set; }

    public GameContext(DbContextOptions<GameContext> options) : base(options) 
    {
         ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
         ChangeTracker.AutoDetectChangesEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ValidationResult>();

        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
            e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(100)");

        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameContext).Assembly);
    }

    public async Task<bool> Commit()
    {
        return await SaveChangesAsync() > 0;
    }
}