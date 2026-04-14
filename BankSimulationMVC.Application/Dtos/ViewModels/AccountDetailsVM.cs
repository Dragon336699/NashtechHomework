namespace BankSimulationMVC.Application.Dtos.ViewModels
{
    public class AccountDetailsVM
    {
        public AccountVM Account { get; set; }
        public DepositVM DepositVM { get; set; }
        public WithDrawVM WithdrawVM { get; set; }
        public TransferVM TransferVM { get; set; }
    }
}
