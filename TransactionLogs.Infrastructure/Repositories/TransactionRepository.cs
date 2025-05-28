using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Domain.Interfaces;
using TransactionLogs.Infrastructure.Data;

namespace TransactionLogs.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionContext context;

        public TransactionRepository(TransactionContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            // Implementation for fetching all transactions from the database  
            var transactions = await context.Transactions.ToListAsync();
            return transactions;
        }
    }
}
