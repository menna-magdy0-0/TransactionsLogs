using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using Rebus.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Infrastructure.Data;

namespace TransactionLogs.Infrastructure.Handlers
{
    public class AuditLogHandler : IHandleMessages<List<Transaction>>
    {
        private readonly TransactionContext _context;
        private readonly ILogger<AuditLogHandler> _logger;

        public AuditLogHandler(
            TransactionContext context,
            ILogger<AuditLogHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Handle(List<Transaction> transactions)
        {
            _logger.LogInformation("Received {Count} audit logs", transactions.Count);

            try
            {
                // Reset IDs for database generation
                foreach (var t in transactions)
                {
                    t.Id = 0;
                }

                await _context.Transactions.AddRangeAsync(transactions);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Saved {Count} audit logs to database", transactions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving audit logs");
                throw; // Enable retry mechanism
            }
        }
    
    }
}
