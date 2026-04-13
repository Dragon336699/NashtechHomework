namespace BankSimulationMVC.ViewModels
{
    public class AccountDetailsVM
    {
        public AccountDto Account { get; set; }
        public DepositVM DepositVM { get; set; }
        public WithDrawVM WithdrawVM { get; set; }
        public TransferVM TransferVM { get; set; }
    }
}
