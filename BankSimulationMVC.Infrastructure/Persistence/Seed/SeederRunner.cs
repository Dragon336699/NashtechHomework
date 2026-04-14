using BankSimulationMVC.Application.Interfaces.Seeds;

namespace BankSimulationMVC.Infrastructure.Persistence.Seed
{
    public class SeederRunner
    {
        private readonly IEnumerable<ISeeder> _seeders;
        public SeederRunner(IEnumerable<ISeeder> seeders)
        {
            _seeders = seeders;
        }

        public async Task RunAsync()
        {
            foreach (var seeder in _seeders)
            {
                await seeder.SeedAsync();
            }
        }
    }
}
