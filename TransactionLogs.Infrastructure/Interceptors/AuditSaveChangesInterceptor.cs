using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
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
        private readonly List<(EntityEntry Entry, EntityState State)> _pendingEntries = [];
        private readonly IBus _bus;
        private readonly ILogger<AuditSaveChangesInterceptor> _logger;

        public AuditSaveChangesInterceptor(
            IBus bus,
            ILogger<AuditSaveChangesInterceptor> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null)
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
                var logs = new List<Transaction>();

                foreach (var (entry, state) in _pendingEntries)
                {
                    try
                    {
                        entry.Reload();
                        logs.Add(new Transaction
                        {
                            TableName = entry.Entity.GetType().Name,
                            OperationType = state.ToString(),
                            PrimaryKeyValue = GetPrimaryKey(entry),
                            EntityData = JsonSerializer.Serialize(entry.Entity),
                            TransactionId = context.Database.CurrentTransaction?.TransactionId.ToString(),
                            TimeStamp = DateTime.UtcNow
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating audit log for {EntityType}", entry.Entity.GetType().Name);
                    }
                }

                if (logs.Count > 0)
                {
                    try
                    {
                        await _bus.Send(logs);
                        _logger.LogInformation("Sent {Count} audit logs to RabbitMQ", logs.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send audit logs to RabbitMQ");
                    }
                }

                _pendingEntries.Clear();
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