using BankSimulationMVC.Application.Dtos.ViewModels;
using FluentValidation;

namespace BankSimulationMVC.Application.Validation.Transactions
{
    public class FilterTransactionValidator : AbstractValidator<TransactionVM>
    {
        public FilterTransactionValidator()
        {
            RuleFor(ac => ac.AccountNumber)
                .NotEmpty().WithMessage("Account number is required");
        }
    }
}
