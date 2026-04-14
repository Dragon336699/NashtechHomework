using BankSimulationMVC.Application.Dtos.Requests;
using FluentValidation;

namespace BankSimulationMVC.Application.Validation.Accounts
{
    public class AccountDtoValidator : AbstractValidator<AccountRequest>
    {
        public AccountDtoValidator()
        {
            RuleFor(ac => ac.AccountNumber)
                .NotEmpty().WithMessage("Account number is required");
            RuleFor(ac => ac.OwnerName)
                .NotEmpty().WithMessage("Owner is required");
            RuleFor(ac => ac.InitialBalance)
                .GreaterThanOrEqualTo(0).WithMessage("Initial balance must be greater than or equal to 0");
        }
    }
}
