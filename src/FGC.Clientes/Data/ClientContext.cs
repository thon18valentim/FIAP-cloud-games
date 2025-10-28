using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using FCG.Core.Data;
using FCG.Clients.Models;

namespace FCG.Clients.Data
{
    public sealed class ClientContext : DbContext, IUnitOfWork
    {
        public DbSet<Client> Clientes { get; set; }
        public DbSet<Address> Enderecos { get; set; }

        public ClientContext(DbContextOptions<ClientContext> options)
            : base(options)
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

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            var isNewTransaction = Database.CurrentTransaction == null;
            var transaction = Database.CurrentTransaction ?? await Database.BeginTransactionAsync();

            try
            {
                var sucesso = await SaveChangesAsync() > 0;
                if (!sucesso) return false;

                if (isNewTransaction)
                    await transaction.CommitAsync();

                return true;
            }
            catch
            {
                if (isNewTransaction)
                    await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
