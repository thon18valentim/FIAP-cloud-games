using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using FCG.Games.Models;
using FCG.Core.Data;

namespace FCG.Games.Data
{
    public class GameContext : DbContext, IUnitOfWork
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

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameContext).Assembly);
        }

        public Task<bool> Commit()
        {
            throw new NotImplementedException();
        }
    }
}
