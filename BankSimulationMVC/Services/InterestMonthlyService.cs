using BankSimulationMVC.Data;
using BankSimulationMVC.Interfaces;

namespace BankSimulationMVC.Services
{
    public class InterestMonthlyService : BackgroundService
    {
        private readonly ILogger<InterestMonthlyService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public InterestMonthlyService(ILogger<InterestMonthlyService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background service has been started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;

                    var nextRun = DateTime.Today.AddDays(1);

                    var delay = nextRun - now;

                    await Task.Delay(delay, stoppingToken);

                    await AddInterestMonthly(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background service");
                }
            }

        }

        public async Task AddInterestMonthly(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            await accountService.ProcessMonthlyInterest();
        }
    }
}
