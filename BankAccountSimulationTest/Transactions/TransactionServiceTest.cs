using BankSimulationMVC.Application.Interfaces.Repositories;
using BankSimulationMVC.Application.Interfaces.UnitOfWork;
using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Services;
using Moq;
using System.Linq.Expressions;

namespace BankAccountSimulation.Test.Transactions
{
    public class TransactionServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly TransactionService _transactionService;
        public TransactionServiceTest()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _transactionService = new TransactionService(_mockUow.Object);
        }

        //GET PAGE TRANSACTION
        [Fact]
        public async Task GetPageTransaction_ShouldReturnTransactions()
        {
            // Arrange
            var transactions = new List<Transaction>
                {
                new Transaction { TransactionId = 1, AccountNumber = "1", Amount = 100 },
                new Transaction { TransactionId = 2, AccountNumber = "2", Amount = 200 },
                new Transaction { TransactionId = 3, AccountNumber = "3", Amount = 300 }
                };

            var mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUow.Setup(x => x.Transactions).Returns(mockTransactionRepo.Object);

            mockTransactionRepo
                .Setup(x => x.GetPageTransaction(1, 10))
                .ReturnsAsync(transactions);

            // Act
            var result = await _transactionService.GetPageTransaction(1, 10);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal(transactions, result);
        }

        [Fact]
        public async Task GetPageTransaction_ShouldReturnEmptyList_WhenNoData()
        {
            // Arrange
            var mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUow.Setup(x => x.Transactions).Returns(mockTransactionRepo.Object);

            mockTransactionRepo
                .Setup(x => x.GetPageTransaction(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Transaction>());

            // Act
            var result = await _transactionService.GetPageTransaction(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        //GET TRANSACTION BY ID
        [Fact]
        public void GetTransactionsById_ShouldReturnCorrectTransactions()
        {
            // Arrange
            var transactions = new List<Transaction>
                {
                new Transaction { TransactionId = 1, AccountNumber = "1", Amount = 100 },
                new Transaction { TransactionId = 2, AccountNumber = "1", Amount = 200 },
                new Transaction { TransactionId = 3, AccountNumber = "2", Amount = 300 }
                };

            var mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUow.Setup(x => x.Transactions).Returns(mockTransactionRepo.Object);

            mockTransactionRepo
                .Setup(x => x.Find(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .Returns((Expression<Func<Transaction, bool>> predicate) =>
                {
                    return transactions.AsQueryable().Where(predicate).ToList();
                });

            // Act
            var result = _transactionService.GetTransactionsByAccountNumber("1");

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetTransactionsById_ShouldReturnEmptyList_WhenNoMatch()
        {
            // Arrange
            var mockTransactionRepo = new Mock<ITransactionRepository>();
            _mockUow.Setup(x => x.Transactions).Returns(mockTransactionRepo.Object);

            mockTransactionRepo
                .Setup(x => x.Find(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .Returns(new List<Transaction>());

            // Act
            var result = _transactionService.GetTransactionsByAccountNumber("1");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

    }
}
