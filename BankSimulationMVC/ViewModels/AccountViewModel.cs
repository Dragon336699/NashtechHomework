using BankSimulationMVC.Models;

namespace BankSimulationMVC.ViewModels
{
    public class AccountViewModel
    {
        public IEnumerable<AccountDto> Accounts { get; set; }
        public AccountDto Account { get; set; }
        public DepositVM DepositVM { get; set; }
        public WithDrawVM WithDrawVM { get; set; }
        public TransferVM TransferVM { get; set; }
    }
}
