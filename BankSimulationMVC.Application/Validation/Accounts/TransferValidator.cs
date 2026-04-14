using BankSimulationMVC.Application.Dtos.ViewModels;
using FluentValidation;

namespace BankSimulationMVC.Application.Validation.Accounts
{
    public class TransferValidator : AbstractValidator<TransferVM>
    {
        public TransferValidator()
        {
            RuleFor(ac => ac.SourceAccountNumber)
                .NotEmpty().WithMessage("Source account number is required");
            RuleFor(ac => ac.DestinationAccountNumber)
                .NotEmpty().WithMessage("Destination account number is required");
            RuleFor(ac => ac.Amount)
                .GreaterThan(0).WithMessage("Transfer money must be greater than 0");
        }
    }
}
