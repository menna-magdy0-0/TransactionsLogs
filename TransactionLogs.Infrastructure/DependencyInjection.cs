using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLogs.Domain.Interfaces;
using TransactionLogs.Infrastructure.Data;
using TransactionLogs.Infrastructure.Repositories;

namespace TransactionLogs.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TransactionContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("cs"),
                b => b.MigrationsAssembly(typeof(TransactionContext).Assembly.FullName)));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            return services;
        }
        //public static void AddInfrastructureServices(IServiceCollection services, IConfiguration configuration)
        //{
        //    // Register your DbContext
        //    services.AddDbContext<TransactionContext>(options =>
        //        options.UseSqlServer(configuration.GetConnectionString("cs")));
        //    // Register repositories
        //    services.AddScoped<IProductRepository, ProductRepository>();
        //    services.AddScoped<IUserRepository, UserRepository>();
        //    services.AddScoped<ITransactionRepository, TransactionRepository>();
        //    // Add other infrastructure services as needed
        //}
    }
}
