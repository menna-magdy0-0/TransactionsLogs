using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Infrastructure.Interceptors;

namespace TransactionLogs.Infrastructure.Data
{
    public class TransactionContext : DbContext
    {
        private readonly AuditSaveChangesInterceptor _auditInterceptor;

        public TransactionContext(DbContextOptions<TransactionContext> options, AuditSaveChangesInterceptor auditInterceptor) : base(options)
        {
            _auditInterceptor = auditInterceptor;
        }
        public TransactionContext(DbContextOptions<TransactionContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Transaction>()
                .Property(e => e.Id)
                .UseIdentityColumn();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("cs");
            }
            
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
       
}
