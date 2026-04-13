using BankSimulationMVC.Interfaces;

namespace BankSimulationMVC.Services
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
        }
    }
}
