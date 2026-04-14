using BankSimulationMVC.Interfaces.Services;
using BankSimulationMVC.Mapper;
using Microsoft.AspNetCore.Mvc;
using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Application.Dtos.ViewModels;
using BankSimulationMVC.Application.Dtos.Responses;
using BankSimulationMVC.Application.Dtos.Requests;

namespace BankSimulationMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            IEnumerable<AccountVM> allAccounts = _accountService.GetAllAccounts();
            AccountPageVM viewModel = new AccountPageVM
            {
                Accounts = allAccounts
            };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var account = await _accountService.GetAccountDetails(id);
            if (account == null) return NotFound();
            var viewModel = new AccountDetailsVM
            {
                Account = account,

                DepositVM = new DepositVM
                {
                    AccountNumber = account.AccountNumber
                },

                WithdrawVM = new WithDrawVM
                {
                    AccountNumber = account.AccountNumber
                },

                TransferVM = new TransferVM
                {
                    SourceAccountNumber = account.AccountNumber
                }
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(AccountRequest accountDto)
        {
            if (!ModelState.IsValid)
            {
                return View(accountDto);
            }

            Account account = accountDto.ToDomain();
            ServiceResult result = await _accountService.CreateAccount(account);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositVM depositViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(" | ",
                    ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                TempData["Error"] = errorMessage;
                return RedirectToAction($"Details", new { id = depositViewModel.AccountNumber });
            }

            ServiceResult result = await _accountService.Deposit(depositViewModel);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction($"Details", new {id = depositViewModel.AccountNumber});
        }

        [HttpPost]
        public async Task<IActionResult> WithDraw(WithDrawVM withdrawViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(" | ",
                    ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                TempData["Error"] = errorMessage;
                return RedirectToAction($"Details", new { id = withdrawViewModel.AccountNumber });
            }

            ServiceResult result = await _accountService.Withdraw(withdrawViewModel);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction($"Details", new { id = withdrawViewModel.AccountNumber });
        }
        [HttpPost]
        public async Task<IActionResult> Transfer(TransferVM transferViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(" | ",
                    ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                TempData["Error"] = errorMessage;
                return RedirectToAction($"Details", new { id = transferViewModel.SourceAccountNumber });
            }

            ServiceResult result = await _accountService.Transfer(transferViewModel);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction($"Details", new { id = transferViewModel.SourceAccountNumber });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAccountStatus(string accountNumber)
        {
            ServiceResult result = await _accountService.ToggleAccountStatus(accountNumber);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
