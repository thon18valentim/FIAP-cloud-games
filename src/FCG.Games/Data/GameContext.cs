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

        public async Task<bool> Commit()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("CreationTime") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("CreatedAt").IsModified = false;
                    entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified && (bool)entry.Property("IsDeleted").CurrentValue)
                {
                    foreach (var property in entry.Properties)
                    {
                        property.IsModified = false;
                    }

                    entry.Property("IsDeleted").IsModified = true;
                    entry.Property("DeletedAt").IsModified = true;
                    entry.Property("DeletedAt").CurrentValue = DateTime.Now;
                }
            }

            var sucesso = await base.SaveChangesAsync() > 0;

            return sucesso;
        }
    }
}
