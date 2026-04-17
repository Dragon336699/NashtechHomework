using BankSimulationMVC.Application.Interfaces.Seeds;

namespace BankSimulationMVC.Infrastructure.Persistence.Seed
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Today => DateTime.Today;
    }
}
