using BankSimulationMVC.Application.Dtos.ViewModels;
using BankSimulationMVC.Application.Interfaces.Repositories;
using BankSimulationMVC.Application.Interfaces.UnitOfWork;
using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Services;
using BankSimulationMVC.Enum;
using Moq;
using System.Linq.Expressions;
using Xunit;
using BankSimulationMVC.Application.Interfaces.Seeds;
namespace BankAccountSimulation.Test.Accounts
{
    public class AccountsServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        private readonly AccountService _accountService;
        public AccountsServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _accountService = new AccountService(_mockUow.Object, _mockDateTimeProvider.Object);
        }

        // GET ACCOUNT DETAIL
        [Fact]
        public async Task GetAccountDetails_ShouldSucceed_WhenAccountExist()
        {
            //Arrange
            var fixedTime = new DateTime(2026, 01, 01);
            string accountNumber = "1";

            Account mockAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
                Status = AccountStatus.Active,
                WithdrawLimit = 10000,
                CreatedAt = fixedTime,
                LastWithdrawDate = fixedTime,
                LastAddInterestMonthlyDate = fixedTime,
                InterestMonthly = 20000
            };

            mockAccount.UpdateBalance(10000);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);
            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(mockAccount);

            //Act
            var existAccount = await _accountService.GetAccountDetails(accountNumber);

            //Assert
            Assert.Equal("1", existAccount.AccountNumber);
            Assert.Equal("Long", existAccount.OwnerName);
            Assert.Equal(AccountStatus.Active, existAccount.Status);
            Assert.Equal(10000, existAccount.Balance);
            Assert.Equal(fixedTime, existAccount.CreatedAt);
        }

        [Fact]
        public async Task GetAccountDetails_ShouldReturnNull_WhenAccountNotFound()
        {
            // Arrange
            string accountNumber = "999";

            var mockRepo = new Mock<IAccountRepository>();

            mockRepo
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync((Account?)null);

            _mockUow.Setup(x => x.Accounts).Returns(mockRepo.Object);

            // Act
            var result = await _accountService.GetAccountDetails(accountNumber);

            // Assert
            Assert.Null(result);
        }

        //GET ALL ACCOUNTS
        [Fact]
        public void GetAllAccounts_ShouldReturnMappedAccountList()
        {
            // Arrange
            var fixedTime = new DateTime(2026, 01, 01);

            var accounts = new List<Account>
        {
        new Account
        {
            AccountNumber = "1",
            OwnerName = "Long",
            Status = AccountStatus.Active,
            CreatedAt = fixedTime
        },
        new Account
        {
            AccountNumber = "2",
            OwnerName = "An",
            Status = AccountStatus.Frozen,
            CreatedAt = fixedTime
        }
        };

            accounts[0].UpdateBalance(10000);
            accounts[1].UpdateBalance(20000);

            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(x => x.GetAll()).Returns(accounts);

            _mockUow.Setup(x => x.Accounts).Returns(mockRepo.Object);

            // Act
            var result = _accountService.GetAllAccounts().ToList();

            // Assert
            Assert.Equal(2, result.Count);

            Assert.Equal("1", result[0].AccountNumber);
            Assert.Equal("Long", result[0].OwnerName);
            Assert.Equal(10000, result[0].Balance);
            Assert.Equal(AccountStatus.Active, result[0].Status);

            Assert.Equal("2", result[1].AccountNumber);
            Assert.Equal("An", result[1].OwnerName);
            Assert.Equal(20000, result[1].Balance);
            Assert.Equal(AccountStatus.Frozen, result[1].Status);
        }

        [Fact]
        public void GetAllAccounts_ShouldReturnEmptyList_WhenNoAccountsExist()
        {
            // Arrange
            var emptyAccounts = new List<Account>();

            var mockRepo = new Mock<IAccountRepository>();
            mockRepo.Setup(x => x.GetAll()).Returns(emptyAccounts);

            _mockUow.Setup(x => x.Accounts).Returns(mockRepo.Object);

            // Act
            var result = _accountService.GetAllAccounts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        //CREATE ACCOUNT
        [Fact]
        public async Task CreateAccount_ShouldSucceed_WhenAccountNotExist()
        {
            //Arrange
            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };

            var mockAccountRepo = new Mock<IAccountRepository>();

            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            //Act
            var result = await _accountService.CreateAccount(newBankAccount);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Create account successfully.", result.Message);
            //mockAccountRepo.Verify(x => x.AddAsync(It.IsAny<Account>()), Times.Once);
            //_mockUow.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_ShouldFail_WhenAccountExist()
        {
            //Arrange
            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long"
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(newBankAccount);

            //Act
            var result = await _accountService.CreateAccount(newBankAccount);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Account exist, cannot create.", result.Message);
        }

        //DEPOSIT
        [Fact]
        public async Task Deposit_ShouldFail_WhenNotExistAccount()
        {
            //Arrange
            var initial = 0;
            var deposit = 1000000;
            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };
            DepositVM depositViewModel = new DepositVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = deposit
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync((Account?)null);


            //Act
            var result = await _accountService.Deposit(depositViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Account not found.", result.Message);

        }

        [Fact]
        public async Task Deposit_ShouldFail_WhenAccountFrozen()
        {
            //Arrange
            var initial = 0;
            var deposit = 1000000;

            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
                Status = AccountStatus.Frozen
            };

            DepositVM depositViewModel = new DepositVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = deposit
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);
            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(newBankAccount);

            //Act
            var result = await _accountService.Deposit(depositViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Your account has been frozen, cannot deposit.", result.Message);

        }

        [Fact]
        public async Task Deposit_ShouldFail_WhenInvalidAmount()
        {
            //Arrange
            var initial = 0;
            var deposit = -100000;

            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };
            DepositVM depositViewModel = new DepositVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = deposit
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);
            mockAccountRepo
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync(newBankAccount);

            //Act
            var result = await _accountService.Deposit(depositViewModel);

            //Assert
            Assert.Equal("Deposit money must be greater than 0.", result.Message);
            Assert.False(result.IsSuccess);

        }

        [Fact]
        public async Task Deposit_ShouldSucceed_WhenValidAmount()
        {
            //Arrange
            var initial = 0;
            var deposit = 1000000;
            var expected = 1000000;

            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };
            DepositVM depositViewModel = new DepositVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = deposit
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();

            mockAccountRepo
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync(newBankAccount);

            _mockUow.Setup(x => x.Accounts)
                    .Returns(mockAccountRepo.Object);

            var mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUow.Setup(x => x.Transactions).Returns(mockTransactionRepo.Object);

            //Act
            var result = await _accountService.Deposit(depositViewModel);

            //Assert
            Assert.Equal(expected, newBankAccount.Balance);
            Assert.Equal("Deposit successfully", result.Message);
            Assert.True(result.IsSuccess);
            //_mockUow.Verify(x => x.CompleteAsync(), Times.Once);
        }

        //WITHDRAW

        [Fact]
        public async Task Withdraw_ShouldFail_WhenNotExistAccount()
        {
            //Arrange
            var initial = 0;
            var withdraw = 1000000;
            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };
            WithDrawVM withDrawViewModel = new WithDrawVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = withdraw
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync((Account?)null);


            //Act
            var result = await _accountService.Withdraw(withDrawViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Account not found.", result.Message);

        }

        [Fact]
        public async Task Withdraw_ShouldFail_WhenInvalidAmount()
        {
            //Arrange
            var initial = 0;
            var withdraw = -1000000;

            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };
            WithDrawVM withdrawViewModel = new WithDrawVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = withdraw
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);
            mockAccountRepo
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync(newBankAccount);

            //Act
            var result = await _accountService.Withdraw(withdrawViewModel);

            //Assert
            Assert.Equal("You have to withdraw money greater than 0.", result.Message);
            Assert.False(result.IsSuccess);

        }

        [Fact]
        public async Task Withdraw_ShouldFail_WhenAccountFrozen()
        {
            //Arrange
            var initial = 0;
            var withdraw = 1000000;
            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
                Status = AccountStatus.Frozen
            };

            WithDrawVM withdrawViewModel = new WithDrawVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = withdraw
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);
            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(newBankAccount);

            //Act
            var result = await _accountService.Withdraw(withdrawViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Your account has been frozen, cannot withdraw.", result.Message);

        }

        [Fact]
        public async Task Withdraw_ShouldFail_WhenAmountGreaterThanBalance()
        {
            //Arrange
            var initial = 0;
            var withdraw = 200000000;
            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };

            WithDrawVM withdrawViewModel = new WithDrawVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = withdraw
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);
            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(newBankAccount);

            //Act
            var result = await _accountService.Withdraw(withdrawViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Your balance is not enough.", result.Message);

        }

        [Fact]
        public async Task Withdraw_ShouldFail_WhenNewBalanceLowerThan100()
        {
            //Arrange
            var initial = 1000000;
            var withdraw = 1000000;
            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };

            WithDrawVM withdrawViewModel = new WithDrawVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = withdraw
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);
            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(newBankAccount);

            //Act
            var result = await _accountService.Withdraw(withdrawViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Minimal balance must be 100.", result.Message);

        }

        [Fact]
        public async Task Withdraw_ShouldFail_WhenWithdrawLimitUsedUp()
        {
            //Arrange
            var initial = 1000000;
            var withdraw = 1000000;
            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
                WithdrawLimit = 0,
                LastWithdrawDate = DateTime.Now
            };

            WithDrawVM withdrawViewModel = new WithDrawVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = withdraw
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);
            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(newBankAccount);

            //Act
            var result = await _accountService.Withdraw(withdrawViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("The withdrawal limit is used up.", result.Message);

        }

        [Fact]
        public async Task Withdraw_ShouldSucceed_WhenValidAmount()
        {
            //Arrange
            var initial = 1000000;
            var withdraw = 500000;
            var expected = 500000;

            Account newBankAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };

            WithDrawVM withdrawViewModel = new WithDrawVM
            {
                AccountNumber = newBankAccount.AccountNumber,
                Amount = withdraw
            };

            newBankAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();

            mockAccountRepo
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync(newBankAccount);

            _mockUow.Setup(x => x.Accounts)
                    .Returns(mockAccountRepo.Object);

            var mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUow.Setup(x => x.Transactions).Returns(mockTransactionRepo.Object);

            //Act
            var result = await _accountService.Withdraw(withdrawViewModel);

            //Assert
            Assert.Equal(expected, newBankAccount.Balance);
            Assert.Equal($"Withdraw successfully, your new balance is {expected}", result.Message);
            Assert.True(result.IsSuccess);
            //_mockUow.Verify(x => x.CompleteAsync(), Times.Once);
        }

        //TRANSFER
        [Fact]
        public async Task Transfer_ShouldFail_WhenSourceAccountNotFound()
        {
            //Arrange
            var initial = 100000;
            var transfer = 1000;
            Account sourceAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
            };

            Account destinationAccount = new Account
            {
                AccountNumber = "2",
                OwnerName = "Minh",
            };

            TransferVM transferViewModel = new TransferVM
            {
                SourceAccountNumber = sourceAccount.AccountNumber,
                DestinationAccountNumber = destinationAccount.AccountNumber,
                Amount = transfer
            };

            sourceAccount.UpdateBalance(initial);
            destinationAccount.UpdateBalance(initial);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync((Account?)null);

            //Act
            var result = await _accountService.Transfer(transferViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Source account not found.", result.Message);

        }

        [Fact]
        public async Task Transfer_ShouldFail_WhenDestinationAccountNotFound()
        {
            //Arrange
            var initialSrcAccount = 10000;
            var transfer = 1000;
            Account sourceAccount = new Account { AccountNumber = "1", OwnerName = "Long" };
            Account? destinationAccount = null;

            TransferVM transferViewModel = new TransferVM
            {
                SourceAccountNumber = sourceAccount.AccountNumber,
                DestinationAccountNumber = "2",
                Amount = transfer
            };

            sourceAccount.UpdateBalance(initialSrcAccount);

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Expression<Func<Account, bool>> predicate) =>
            {
                var f = predicate.Compile();
                var accounts = new[] { sourceAccount, destinationAccount }
                    .Where(a => a != null);
                return accounts.FirstOrDefault(a => f(a));
            });

            //Act
            var result = await _accountService.Transfer(transferViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Destination account not found.", result.Message);

        }

        [Fact]
        public async Task Transfer_ShouldFail_WhenSourceAccountFrozen()
        {
            //Arrange
            var transfer = 1000;

            Account sourceAccount = new Account { AccountNumber = "1", OwnerName = "Long", Status = AccountStatus.Frozen };
            Account destinationAccount = new Account { AccountNumber = "2", OwnerName = "Minh" };
            TransferVM transferViewModel = new TransferVM
            {
                SourceAccountNumber = sourceAccount.AccountNumber,
                DestinationAccountNumber = destinationAccount.AccountNumber,
                Amount = transfer
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Expression<Func<Account, bool>> predicate) =>
            {
                var f = predicate.Compile();
                var accounts = new[] { sourceAccount, destinationAccount }
                    .Where(a => a != null);
                return accounts.FirstOrDefault(a => f(a));
            });

            //Act
            var result = await _accountService.Transfer(transferViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Source account has been frozen.", result.Message);

        }

        [Fact]
        public async Task Transfer_ShouldFail_WhenInvalidAmount()
        {
            //Arrange
            var transfer = -100;

            Account sourceAccount = new Account { AccountNumber = "1", OwnerName = "Long" };
            Account destinationAccount = new Account { AccountNumber = "2", OwnerName = "Minh" };
            TransferVM transferViewModel = new TransferVM
            {
                SourceAccountNumber = "1",
                DestinationAccountNumber = "2",
                Amount = transfer
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Expression<Func<Account, bool>> predicate) =>
            {
                var f = predicate.Compile();
                var accounts = new[] { sourceAccount, destinationAccount }
                    .Where(a => a != null);
                return accounts.FirstOrDefault(a => f(a));
            });

            //Act
            var result = await _accountService.Transfer(transferViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Amount must be greater than 0.", result.Message);

        }

        [Fact]
        public async Task Transfer_ShouldFail_WhenAmountGreaterThanBalance()
        {
            //Arrange
            var initial = 1000;
            var transfer = 2000;
            Account sourceAccount = new Account { AccountNumber = "1", OwnerName = "Long" };
            sourceAccount.UpdateBalance(initial);
            Account destinationAccount = new Account { AccountNumber = "2", OwnerName = "Minh" };
            TransferVM transferViewModel = new TransferVM
            {
                SourceAccountNumber = "1",
                DestinationAccountNumber = "2",
                Amount = transfer
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Expression<Func<Account, bool>> predicate) =>
            {
                var f = predicate.Compile();
                var accounts = new[] { sourceAccount, destinationAccount }
                    .Where(a => a != null);
                return accounts.FirstOrDefault(a => f(a));
            });

            //Act
            var result = await _accountService.Transfer(transferViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Your balance is not enough.", result.Message);
            _mockUow.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Transfer_ShouldFail_WhenWithdrawLimitUsedUp()
        {
            //Arrange
            var initial = 10000;
            var transfer = 5000;
            Account sourceAccount = new Account { AccountNumber = "1", OwnerName = "Long", WithdrawLimit = 500, LastWithdrawDate = DateTime.Now };
            sourceAccount.UpdateBalance(initial);
            Account destinationAccount = new Account { AccountNumber = "2", OwnerName = "Minh" };
            TransferVM transferViewModel = new TransferVM
            {
                SourceAccountNumber = "1",
                DestinationAccountNumber = "2",
                Amount = transfer
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Expression<Func<Account, bool>> predicate) =>
            {
                var f = predicate.Compile();
                var accounts = new[] { sourceAccount, destinationAccount }
                    .Where(a => a != null);
                return accounts.FirstOrDefault(a => f(a));
            });

            //Act
            var result = await _accountService.Transfer(transferViewModel);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("The withdrawal limit is used up.", result.Message);

        }

        [Fact]
        public async Task Transfer_ShouldSucceed_WhenValid()
        {
            //Arrange
            var initialSourceAccount = 10000;
            var initialDestinationAccount = 0;
            var transfer = 5000;
            var expectedBalanceSrcAccount = 5000;
            var expectedBalanceDesAccount = 5000;

            Account sourceAccount = new Account { AccountNumber = "1", OwnerName = "Long" };
            sourceAccount.UpdateBalance(initialSourceAccount);
            Account destinationAccount = new Account { AccountNumber = "2", OwnerName = "Minh" };
            destinationAccount.UpdateBalance(initialDestinationAccount);
            TransferVM transferViewModel = new TransferVM
            {
                SourceAccountNumber = sourceAccount.AccountNumber,
                DestinationAccountNumber = destinationAccount.AccountNumber,
                Amount = transfer
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            var mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUow.Setup(x => x.Transactions).Returns(mockTransactionRepo.Object);

            mockAccountRepo
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Expression<Func<Account, bool>> predicate) =>
            {
                var f = predicate.Compile();
                var accounts = new[] { sourceAccount, destinationAccount }
                    .Where(a => a != null);
                return accounts.FirstOrDefault(a => f(a));
            });

            //Act
            var result = await _accountService.Transfer(transferViewModel);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Transfer successful", result.Message);
            Assert.Equal(expectedBalanceSrcAccount, sourceAccount.Balance);
            Assert.Equal(expectedBalanceDesAccount, destinationAccount.Balance);
            _mockUow.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Transfer_ShouldFail_WhenExceptionThrown()
        {
            // Arrange
            var sourceAccount = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long"
            };
            sourceAccount.UpdateBalance(10000);

            var destinationAccount = new Account
            {
                AccountNumber = "2",
                OwnerName = "A"
            };

            var transferVM = new TransferVM
            {
                SourceAccountNumber = "1",
                DestinationAccountNumber = "2",
                Amount = 1000
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
            .ReturnsAsync((Expression<Func<Account, bool>> predicate) =>
            {
                var f = predicate.Compile();
                var accounts = new[] { sourceAccount, destinationAccount }
                    .Where(a => a != null);
                return accounts.FirstOrDefault(a => f(a));
            });

            _mockUow
                .Setup(x => x.CompleteAsync())
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _accountService.Transfer(transferVM);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Transfer failed", result.Message);
        }

        //TOGGLE ACCOUNT STATUS
        [Fact]
        public async Task ToggleAccountStatus_ShouldFail_WhenAccountNotFound()
        {
            // Arrange
            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync((Account?)null);

            // Act
            var result = await _accountService.ToggleAccountStatus("1");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Source account not found.", result.Message);

        }

        [Fact]
        public async Task ToggleAccountStatus_ShouldFreeze_WhenAccountIsActive()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
                Status = AccountStatus.Active
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.ToggleAccountStatus("1");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(AccountStatus.Frozen, account.Status);
            Assert.Contains("Froze account 1 successfully.", result.Message);
            //_mockUow.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task ToggleAccountStatus_ShouldActivate_WhenAccountIsFrozen()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
                Status = AccountStatus.Frozen
            };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync(account);

            // Act
            var result = await _accountService.ToggleAccountStatus("1");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(AccountStatus.Active, account.Status);
            Assert.Contains("Activated account 1 successfully.", result.Message);
            //_mockUow.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessMonthlyInterest_ShouldAccumulateInterest_WhenNotDay10()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
                InterestMonthly = 0
            };

            account.UpdateBalance(36500);

            var accounts = new List<Account> { account };

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(accounts);

            // Act
            await _accountService.ProcessMonthlyInterest();

            // Assert
            Assert.True(account.InterestMonthly > 0);
            Assert.Equal(36500, account.Balance);
            //_mockUow.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessMonthlyInterest_ShouldResetAndAddBalance_WhenDay10()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "1",
                OwnerName = "Long",
                InterestMonthly = 100
            };

            account.UpdateBalance(1000);

            var fixedTime = new DateTime(2026, 4, 10);

            var accounts = new List<Account> { account };

            _mockDateTimeProvider.Setup(x => x.Today)
                .Returns(new DateTime(2026, 4, 10));

            var mockAccountRepo = new Mock<IAccountRepository>();
            _mockUow.Setup(x => x.Accounts).Returns(mockAccountRepo.Object);

            mockAccountRepo
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(accounts);

            // Act
            await _accountService.ProcessMonthlyInterest();

            // Assert
            Assert.Equal(0, account.InterestMonthly);
            Assert.True(account.Balance > 1000);
        }

    }
}
