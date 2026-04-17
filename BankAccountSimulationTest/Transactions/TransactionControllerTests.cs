using BankSimulationMVC.Application.Dtos.ViewModels;
using BankSimulationMVC.Controllers;
using BankSimulationMVC.Data;
using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Enum;
using BankSimulationMVC.Infrastructure.UnitOfWork;
using BankSimulationMVC.Interfaces.Services;
using BankSimulationMVC.Repositories;
using BankSimulationMVC.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;

namespace BankAccountSimulation.Test.Transactions
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new TransactionController(_mockTransactionService.Object);
        }

        private BankDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<BankDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new BankDbContext(options);
        }

        private TransactionController CreateController(BankDbContext context)
        {
            var accountRepo = new AccountRepository(context);
            var transactionRepo = new TransactionRepository(context);

            var uow = new UnitOfWork(context, accountRepo, transactionRepo);

            var service = new TransactionService(uow);

            return new TransactionController(service);
        }

        [Fact]
        public async Task Index_ShouldReturnAll_WhenTypeIsNull()
        {
            var context = CreateDbContext();

            context.Transactions.AddRange(
                new Transaction { AccountNumber = "1", Type = TransactionType.Deposit },
                new Transaction { AccountNumber = "2", Type = TransactionType.Withdraw }
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Index(null, 1) as ViewResult;
            var model = result.Model as TransactionVM;

            Assert.Equal(2, model.Transactions.Count());
        }

        [Fact]
        public async Task Index_ShouldFilterByType()
        {
            var context = CreateDbContext();

            context.Transactions.AddRange(
                new Transaction { AccountNumber = "1", Type = TransactionType.Deposit },
                new Transaction { AccountNumber = "2", Type = TransactionType.Withdraw }
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Index("deposit", 1) as ViewResult;
            var model = result.Model as TransactionVM;

            Assert.Single(model.Transactions);
            Assert.Equal(TransactionType.Deposit, model.Transactions.First().Type);
        }

        [Fact]
        public async Task Index_ShouldApplyPaging()
        {
            var context = CreateDbContext();

            for (int i = 0; i < 25; i++)
            {
                context.Transactions.Add(new Transaction {AccountNumber = i.ToString(), Type = TransactionType.Deposit });
            }
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Index(null, 2) as ViewResult;
            var model = result.Model as TransactionVM;

            Assert.Equal(10, model.Transactions.Count());
        }

        [Fact]
        public async Task Index_ShouldSetViewBagCorrectly()
        {
            var context = CreateDbContext();

            for (int i = 0; i < 15; i++)
            {
                context.Transactions.Add(new Transaction { AccountNumber = i.ToString(), Type = TransactionType.Deposit });
            }
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Index("deposit", 2) as ViewResult;

            Assert.Equal(2, controller.ViewBag.Page);
            Assert.Equal(2, controller.ViewBag.TotalPages);
            Assert.Equal("deposit", controller.ViewBag.Type);
        }

        [Fact]
        public async Task Index_ShouldReturnEmpty_WhenPageTooLarge()
        {
            var context = CreateDbContext();

            for (int i = 0; i < 5; i++)
            {
                context.Transactions.Add(new Transaction {AccountNumber = i.ToString(), Type = TransactionType.Deposit });
            }
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Index(null, 10) as ViewResult;
            var model = result.Model as TransactionVM;

            Assert.Empty(model.Transactions);
        }

        [Fact]
        public async Task Index_ShouldReturnEmpty_WhenTypeInvalid()
        {
            var context = CreateDbContext();

            context.Transactions.Add(new Transaction {AccountNumber = "1", Type = TransactionType.Deposit });
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Index("abc", 1) as ViewResult;
            var model = result.Model as TransactionVM;

            Assert.Empty(model.Transactions);
        }

        [Fact]
        public void History_ShouldReturnViewWithTransactions()
        {
            // Arrange
            var accountNumber = "12345";
            var transactions = new List<Transaction>
            {
                new Transaction { TransactionId = 1, AccountNumber = accountNumber, Amount = 100 },
                new Transaction { TransactionId = 2, AccountNumber = accountNumber, Amount = 200 }
            };

            _mockTransactionService.Setup(s => s.GetTransactionsByAccountNumber(accountNumber))
                                   .Returns(transactions);

            // Act
            var result = _controller.History(accountNumber);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Transaction>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void History_ShouldReturnViewWithAccountNumber_WhenModelStateInvalid()
        {
            // Arrange
            var accountNumber = "12345";
            _controller.ModelState.AddModelError("Error", "Model state is invalid");

            // Act
            var result = _controller.History(accountNumber);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
