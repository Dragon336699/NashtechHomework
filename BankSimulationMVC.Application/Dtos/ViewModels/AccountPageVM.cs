namespace BankSimulationMVC.Application.Dtos.ViewModels
{
    public class AccountPageVM
    {
        public IEnumerable<AccountVM> Accounts { get; set; }
        public AccountVM Account { get; set; }
        public DepositVM DepositVM { get; set; }
        public WithDrawVM WithDrawVM { get; set; }
        public TransferVM TransferVM { get; set; }
    }
}
