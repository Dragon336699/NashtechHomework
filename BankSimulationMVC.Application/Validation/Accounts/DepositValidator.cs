using BankSimulationMVC.Application.Dtos.ViewModels;
using FluentValidation;

namespace BankSimulationMVC.Application.Validation.Accounts
{
    public  class DepositValidator : AbstractValidator<DepositVM>
    {
        public DepositValidator()
        {
            RuleFor(ac => ac.AccountNumber)
                .NotEmpty().WithMessage("Account number is required");
            RuleFor(ac => ac.Amount)
                .GreaterThan(0).WithMessage("Deposit amount must be greater than 0");
        }
    }
}
