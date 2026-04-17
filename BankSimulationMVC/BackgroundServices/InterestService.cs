using BankSimulationMVC.Interfaces.Services;

namespace BankSimulationMVC.BackgroundServices
{
    public class InterestService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public InterestService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWorkOnce();

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        public async Task DoWorkOnce()
        {
            using var scope = _serviceProvider.CreateScope();

            var accountService = scope.ServiceProvider
                .GetRequiredService<IAccountService>();

            await accountService.ProcessMonthlyInterest();
        }
    }
}
