using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using TransactionLogs.Infrastructure.Interceptors;
namespace TransactionLogs.Infrastructure.Data
{
    public class TransactionContextFactory : IDesignTimeDbContextFactory<TransactionContext>
    {
        public TransactionContext CreateDbContext(string[] args)
        {
            // Manually resolve the path to the web project
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../TransactionLogs.web");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("cs");

            var optionsBuilder = new DbContextOptionsBuilder<TransactionContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var auditInterceptor = new AuditSaveChangesInterceptor();

            return new TransactionContext(optionsBuilder.Options, auditInterceptor);
        }
    }
}
