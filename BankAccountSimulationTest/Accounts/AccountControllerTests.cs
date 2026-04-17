using BankSimulationMVC.Application.Dtos.Requests;
using BankSimulationMVC.Application.Dtos.Responses;
using BankSimulationMVC.Application.Dtos.ViewModels;
using BankSimulationMVC.Controllers;
using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace BankAccountSimulation.Test.Accounts
{
    public class AccountControllerTests
    {
        private readonly AccountController _controller;
        private readonly Mock<IAccountService> _mockAccountService;
        public AccountControllerTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AccountController(_mockAccountService.Object);
        }

        //GET ACCOUNT DETAILS
        [Fact]
        public async Task Details_ShouldReturnNotFound_WhenIdIsNull()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ShouldReturnNotFound_WhenIdIsEmpty()
        {
            // Act
            var result = await _controller.Details("");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ShouldReturnNotFound_WhenAccountNotFound()
        {
            // Arrange
            _mockAccountService
                .Setup(x => x.GetAccountDetails("1"))
                .ReturnsAsync((AccountVM?)null);

            // Act
            var result = await _controller.Details("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ShouldReturnViewWithViewModel_WhenSuccess()
        {
            // Arrange
            var account = new AccountVM
            {
                AccountNumber = "1",
                OwnerName = "Long"
            };

            _mockAccountService
                .Setup(x => x.GetAccountDetails("1"))
                .ReturnsAsync(account);

            // Act
            var result = await _controller.Details("1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<AccountDetailsVM>(viewResult.Model);

            Assert.Equal("1", model.Account.AccountNumber);
            Assert.Equal("1", model.DepositVM.AccountNumber);
            Assert.Equal("1", model.WithdrawVM.AccountNumber);
            Assert.Equal("1", model.TransferVM.SourceAccountNumber);
        }

        [Fact]
        public async Task Create_ShouldReturnView_WhenModelStateInvalid()
        {
            // Arrange
            var dto = new AccountRequest { AccountNumber = null, OwnerName = "Long", InitialBalance = 0 };

            _controller.ModelState.AddModelError("AccountNumber", "Required");

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(dto, viewResult.Model);
        }

        [Fact]
        public async Task Create_ShouldRedirectAndSetSuccessTempData_WhenSuccess()
        {
            //Arrange
            var dto = new AccountRequest { AccountNumber = "1", OwnerName = "Long", InitialBalance = 0 };

            _mockAccountService.Setup(x => x.CreateAccount(It.IsAny<Account>())).ReturnsAsync(new ServiceResult
            {
                IsSuccess = true,
                Message = "Create account successfully."
            });

            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            var result = await _controller.Create(dto);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Create", redirectResult.ActionName);
            Assert.Equal("Create account successfully.", _controller.TempData["Success"]);
        }

        //DEPOSIT
        [Fact]
        public async Task Deposit_ShouldReturnView_WhenModelStateInvalid()
        {
            //Arrange
            var depositVM = new DepositVM { AccountNumber = "1", Amount = -100 };

            _controller.ModelState.AddModelError("Amount", "Invalid amount");
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            var result = await _controller.Deposit(depositVM);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("1", redirectResult.RouteValues["id"]);
            Assert.Contains("Invalid amount", _controller.TempData["Error"].ToString());
        }

        [Fact]
        public async Task Deposit_ShouldRedirectAndSetTempDataSuccess_WhenSuccess()
        {
            //Arrange
            var depositVM = new DepositVM { AccountNumber = "1", Amount = 100 };
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockAccountService.Setup(x => x.Deposit(It.IsAny<DepositVM>())).ReturnsAsync(new ServiceResult
            {
                IsSuccess = true,
                Message = "Deposit successfully"
            });
            //Act
            var result = await _controller.Deposit(depositVM);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("1", redirectResult.RouteValues["id"]);
            Assert.Equal("Deposit successfully", _controller.TempData["Success"].ToString());
        }

        // WITHDRAW
        [Fact]
        public async Task WithDraw_ShouldReturnRedirectToAction_WhenModelStateInvalid()
        {
            //Arrange
            var withdrawVM = new WithDrawVM { AccountNumber = "1", Amount = -100 };

            _controller.ModelState.AddModelError("Amount", "Invalid amount");
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            var result = await _controller.WithDraw(withdrawVM);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("1", redirectResult.RouteValues["id"]);
            Assert.Contains("Invalid amount", _controller.TempData["Error"].ToString());
        }

        [Fact]
        public async Task WithDraw_ShouldRedirectAndSetTempDataSuccess_WhenSuccess()
        {
            //Arrange
            var withdrawVM = new WithDrawVM { AccountNumber = "1", Amount = 100 };
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockAccountService.Setup(x => x.Withdraw(It.IsAny<WithDrawVM>())).ReturnsAsync(new ServiceResult
            {
                IsSuccess = true,
                Message = "Withdraw successfully"
            });
            
            //Act
            var result = await _controller.WithDraw(withdrawVM);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("1", redirectResult.RouteValues["id"]);
            Assert.Equal("Withdraw successfully", _controller.TempData["Success"].ToString());
        }

        // TRANSFER
        [Fact]
        public async Task Transfer_ShouldReturnRedirectToAction_WhenModelStateInvalid()
        {
            //Arrange
            var transferVM = new TransferVM { SourceAccountNumber = "1", DestinationAccountNumber = "2", Amount = -100 };

            _controller.ModelState.AddModelError("Amount", "Invalid amount");
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            //Act
            var result = await _controller.Transfer(transferVM);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("1", redirectResult.RouteValues["id"]);
            Assert.Contains("Invalid amount", _controller.TempData["Error"].ToString());
        }

        [Fact]
        public async Task Transfer_ShouldRedirectAndSetTempDataSuccess_WhenSuccess()
        {
            //Arrange
            var transferVM = new TransferVM { SourceAccountNumber = "1", DestinationAccountNumber = "2", Amount = 100 };
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockAccountService.Setup(x => x.Transfer(It.IsAny<TransferVM>())).ReturnsAsync(new ServiceResult
            {
                IsSuccess = true,
                Message = "Transfer successfully"
            });
            
            //Act
            var result = await _controller.Transfer(transferVM);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectResult.ActionName);
            Assert.Equal("1", redirectResult.RouteValues["id"]);
            Assert.Equal("Transfer successfully", _controller.TempData["Success"].ToString());
        }

        // TOGGLE ACCOUNT STATUS
        [Fact]
        public async Task ToggleAccountStatus_ShouldReturnNotFound_WhenAccountNumberIsNull()
        {
            //Act
            var result = await _controller.ToggleAccountStatus(null);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ToggleAccountStatus_ShouldReturnNotFound_WhenAccountNumberIsEmpty()
        {
            //Act
            var result = await _controller.ToggleAccountStatus("");

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ToggleAccountStatus_ShouldRedirectAndSetTempDataSuccess_WhenSuccess()
        {
            //Arrange
            var accountNumber = "1";
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _mockAccountService.Setup(x => x.ToggleAccountStatus(It.IsAny<string>())).ReturnsAsync(new ServiceResult
            {
                IsSuccess = true,
                Message = "Toggle status successfully"
            });
            
            //Act
            var result = await _controller.ToggleAccountStatus(accountNumber);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Toggle status successfully", _controller.TempData["Success"].ToString());
        }
    }
}
