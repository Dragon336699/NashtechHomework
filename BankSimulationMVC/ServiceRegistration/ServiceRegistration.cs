using BankSimulationMVC.Application.Interfaces.Repositories;
using BankSimulationMVC.Application.Interfaces.Seeds;
using BankSimulationMVC.Application.Interfaces.UnitOfWork;
using BankSimulationMVC.Infrastructure.Persistence.Seed;
using BankSimulationMVC.Infrastructure.UnitOfWork;
using BankSimulationMVC.Interfaces.Services;
using BankSimulationMVC.Repositories;

namespace BankSimulationMVC.Services
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddScoped<ISeeder, AccountSeeder>();

            services.AddScoped<SeederRunner>();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        }
    }
}
