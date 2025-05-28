
using Microsoft.EntityFrameworkCore;
using TransactionLogs.Domain.Interfaces;
using TransactionLogs.Infrastructure.Data;
using TransactionLogs.Infrastructure.Interceptors;
using TransactionLogs.Infrastructure.Repositories;

namespace TransactionLogs.web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<TransactionContext>((sp, options) =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
                options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });

            // Register the interceptor as a singleton
            builder.Services.AddSingleton<AuditSaveChangesInterceptor>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

            //builder.Services.AddScoped<AuditSaveChangesInterceptor>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
