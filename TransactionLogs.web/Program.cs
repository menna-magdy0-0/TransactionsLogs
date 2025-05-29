
using Microsoft.EntityFrameworkCore;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using TransactionLogs.Domain.Entities;
using TransactionLogs.Domain.Interfaces;
using TransactionLogs.Infrastructure.Data;
using TransactionLogs.Infrastructure.Handlers;
using TransactionLogs.Infrastructure.Interceptors;
using TransactionLogs.Infrastructure.Repositories;
using TransactionLogs.Infrastructure.Services;

namespace TransactionLogs.web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Add logging
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            // Configure RabbitMQ
            var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
            var connectionString = $"amqp://{rabbitConfig["Username"]}:{rabbitConfig["Password"]}@{rabbitConfig["Host"]}";

            // Configure Rebus
            builder.Services.AddRebus(configure => configure
                .Logging(l => l.Console())
                .Transport(t => t.UseRabbitMq(connectionString, "audit-log-queue"))
                .Routing(r => r.TypeBased()
                    .Map<List<Transaction>>("audit-log-queue"))
            );
            //builder.Services.AddRebus(configure => configure
            //    .Logging(l => l.Console())  // Add logging
            //    .Transport(t => t.UseRabbitMq(
            //        $"amqp://{builder.Configuration["RabbitMQ:Username"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:Host"]}",
            //        "audit-log-queue"))
            //    .Routing(r => r.TypeBased()
            //        .Map<List<Transaction>>("audit-log-queue"))  // Map specific message type
            //);
            // Register handler
            builder.Services.AddTransient<IHandleMessages<List<Transaction>>, AuditLogHandler>();

            // Add DbContext with interceptor
            builder.Services.AddDbContext<TransactionContext>((sp, options) =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
                options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });

            builder.Services.AddControllers();
            
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<AuditSaveChangesInterceptor>();

            // Register Rebus handler
            builder.Services.AddTransient<IHandleMessages<List<Transaction>>, AuditLogHandler>();

            // Add background service
            builder.Services.AddHostedService<RebusBackgroundService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            // Apply database migrations
            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<TransactionContext>();
            //    db.Database.Migrate();
            //}

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
