using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Infrastructure.Data;

namespace TransactionLogs.Infrastructure.Interceptors
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly List<(EntityEntry Entry, EntityState State)> _pendingEntries = new();
        private bool _isSavingLogs = false;

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (_isSavingLogs || eventData.Context == null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            foreach (var entry in eventData.Context.ChangeTracker.Entries()
                .Where(e => e.Entity.GetType() != typeof(Transaction) &&
                            (e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)))
            {
                _pendingEntries.Add((entry, entry.State));
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override async ValueTask<int> SavedChangesAsync(
                        SaveChangesCompletedEventData eventData,
                        int result,
                        CancellationToken cancellationToken = default)
        {
            if (_pendingEntries.Count > 0 && eventData.Context is DbContext context)
            {
                var pendingLogs = new List<Transaction>();

                foreach (var (entry, state) in _pendingEntries)
                {
                    entry.Reload(); // Refresh to get the final ID
                    pendingLogs.Add(new Transaction
                    {
                        TableName = entry.Entity.GetType().Name,
                        OperationType = state.ToString(),
                        PrimaryKeyValue = GetPrimaryKey(entry),
                        EntityData = JsonSerializer.Serialize(entry.Entity),
                        TransactionId = context.Database.CurrentTransaction?.TransactionId.ToString(),
                        TimeStamp = DateTime.UtcNow
                    });
                }

                try
                {
                    _isSavingLogs = true;

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

                    var optionsBuilder = new DbContextOptionsBuilder<TransactionContext>()
                        .UseSqlServer(configuration.GetConnectionString("cs"));

                    // Use the parameterless constructor
                    await using var auditContext = new TransactionContext(optionsBuilder.Options);
                    auditContext.Transactions.AddRange(pendingLogs);
                    await auditContext.SaveChangesAsync(cancellationToken);
                }
                finally
                {
                    _isSavingLogs = false;
                    _pendingEntries.Clear();
                }
            }

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private static string GetPrimaryKey(EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            return string.Join(",", key?.Properties.Select(p => entry.Property(p.Name).CurrentValue?.ToString()) ?? []);
        }
    }
}