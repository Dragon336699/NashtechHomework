using BankSimulationMVC.Data;
using BankSimulationMVC.Interfaces;
using BankSimulationMVC.Mapper;
using BankSimulationMVC.Models;
using BankSimulationMVC.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.Threading.Tasks;

namespace BankSimulationMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly decimal yearInterest = 0.08m;
        private readonly BankDbContext _context;
        public AccountService(BankDbContext context)
        {
            _context = context;
        }

        public async Task<AccountDto?> GetAccountDetails(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(account => account.AccountNumber == accountNumber);
            var accountDto = account?.ToDto();
            return accountDto;
        }

        public IEnumerable<AccountDto> GetAllAccounts()
        {
            var accounts = _context.Accounts.ToList();
            return accounts.Select(a => a.ToDto());
        }

        public void CreateAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public async Task<ServiceResult> Deposit(DepositVM depositViewModel)
        {
            Account? account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == depositViewModel.AccountNumber);
            if (account == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Account not found."
                };
            }

            if (depositViewModel.Amount <= 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Deposit money must be greater than 0"
                };
            }

            if (account.Status == Enum.AccountStatus.Frozen)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Your account has been frozen, cannot deposit."
                };
            }

            decimal currentBalance = account.Balance;
            decimal newBalance = currentBalance + depositViewModel.Amount;

            account.UpdateBalance(newBalance);
            Transaction transaction = new Transaction
            {
                AccountNumber = account.AccountNumber,
                Amount = depositViewModel.Amount,
                TransactionDate = DateTime.Now,
                Type = Enum.TransactionType.Deposit
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = "Deposit successfully"
            };
        }

        public async Task<ServiceResult> Withdraw(WithDrawVM withdrawViewModel)
        {
            Account? account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == withdrawViewModel.AccountNumber);
            if (account == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Account not found."
                };
            }

            if (withdrawViewModel.Amount <= 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "You have to withdraw money greater than 0."
                };
            }

            decimal currentBalance = account.Balance;
            decimal newBalance = currentBalance - withdrawViewModel.Amount;

            if (account.Status == Enum.AccountStatus.Frozen)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Your account has been frozen, cannot withdraw."
                };
            }

            if (newBalance < 0)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Your balance is not enough."
                };
            }

            if (newBalance < 100)
            {
                return new ServiceResult { IsSuccess = false, Message = "Minimal balace must be 100." };
            }

            if (DateTime.Now.Day > account.LastWithdrawDate.Day)
            {
                account.ResetWithdrawLimit();
            }

            if (account.WithdrawLimit - withdrawViewModel.Amount < 0)
            {
                return new ServiceResult { IsSuccess = false, Message = "The withdrawal limit is used up." };
            }


            account.UpdateBalance(newBalance);

            account.WithdrawLimit -= withdrawViewModel.Amount;
            account.LastWithdrawDate = DateTime.Now;

            Transaction transaction = new Transaction
            {
                AccountNumber = account.AccountNumber,
                Amount = withdrawViewModel.Amount,
                TransactionDate = DateTime.Now,
                Type = Enum.TransactionType.Withdraw
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return new ServiceResult { IsSuccess = false, Message = $"Withdraw successfully, your new balance is {newBalance}" };
        }

        public async Task<ServiceResult> Transfer(TransferVM transferViewModel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Account? sourceAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transferViewModel.SourceAccountNumber);
                if (sourceAccount == null)
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = "Source account not found."
                    };
                }

                Account? destinationAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transferViewModel.DestinationAccountNumber);
                if (destinationAccount == null)
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = "Destination account not found."
                    };
                }

                if (DateTime.Now.Day > sourceAccount.LastWithdrawDate.Day)
                {
                    sourceAccount.ResetWithdrawLimit();
                }

                var validation = ValidateTransfer(sourceAccount, destinationAccount, transferViewModel.Amount);
                if (validation != null)
                    return validation;

                sourceAccount.UpdateBalance(sourceAccount.Balance - transferViewModel.Amount);
                destinationAccount.UpdateBalance(destinationAccount.Balance + transferViewModel.Amount);

                sourceAccount.WithdrawLimit -= transferViewModel.Amount;
                sourceAccount.LastWithdrawDate = DateTime.Now;

                Transaction logTransactionSource = new Transaction
                {
                    AccountNumber = sourceAccount.AccountNumber,
                    Amount = transferViewModel.Amount,
                    TransactionDate = DateTime.Now,
                    Type = Enum.TransactionType.Transfer,
                    Description = "Transfer out"
                };

                Transaction logTransactionDes = new Transaction
                {
                    AccountNumber = sourceAccount.AccountNumber,
                    Amount = transferViewModel.Amount,
                    TransactionDate = DateTime.Now,
                    Type = Enum.TransactionType.Transfer,
                    Description = "Transfer In"
                };

                _context.Transactions.Add(logTransactionSource);
                _context.Transactions.Add(logTransactionDes);


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = "Transfer successful"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Transfer failed"
                };
            }
        }

        public async Task<ServiceResult> ToggleAccountStatus(string accountNumber)
        {
            Account? account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "Source account not found."
                };
            }

            account.Status = account.Status == Enum.AccountStatus.Active ? Enum.AccountStatus.Frozen : Enum.AccountStatus.Active;
            var action = account.Status == Enum.AccountStatus.Frozen ? "Froze" : "Activated";
            await _context.SaveChangesAsync();
            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"{action} account {accountNumber} successfully."
            };
        }

        public async Task ProcessMonthlyInterest()
        {
            var accounts = await _context.Accounts.ToListAsync();
            var today = DateTime.Today;

            foreach (var account in accounts)
            {
                var dailyInterest = account.Balance * yearInterest / 365;
                account.InterestMonthly += dailyInterest;

                if (today.Day == 10)
                {
                    account.UpdateBalance(account.Balance + account.InterestMonthly);
                    account.InterestMonthly = 0;
                }
            }

            await _context.SaveChangesAsync();
        }

        private ServiceResult? ValidateTransfer(Account source, Account destination, decimal amount)
        {
            if (source == null)
                return Fail("Source account not found.");

            if (destination == null)
                return Fail("Destination account not found.");

            if (source.Status == Enum.AccountStatus.Frozen)
                return Fail("Source account has been frozen.");

            if (amount <= 0)
                return Fail("Amount must be greater than 0.");

            if (source.Balance < amount)
                return Fail("Your balance is not enough.");
            if (source.WithdrawLimit - amount < 0)
            {
                return new ServiceResult { IsSuccess = false, Message = "The withdrawal limit is used up." };
            }

            return null;
        }

        private ServiceResult Fail(string message)
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
