using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace TransactionLogs.Infrastructure.Services
{
    public class RebusBackgroundService : BackgroundService
    {
        private readonly IBus _bus;
        private readonly ILogger<RebusBackgroundService> _logger;

        public RebusBackgroundService(
            IBus bus,
            ILogger<RebusBackgroundService> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Rebus background service started");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
            _logger.LogInformation("Rebus background service stopped");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _bus.Dispose(); 
            await base.StopAsync(cancellationToken);
        }
    }
}
