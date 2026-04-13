using BankSimulationMVC.Interfaces;
using BankSimulationMVC.Mapper;
using BankSimulationMVC.Models;
using BankSimulationMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            IEnumerable<AccountDto> allAccounts = _accountService.GetAllAccounts();
            AccountViewModel viewModel = new AccountViewModel
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
        public async Task<IActionResult> Create(AccountDto accountDto)
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
            ServiceResult result = await _accountService.Deposit(depositViewModel);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction($"Details", new {id = depositViewModel.AccountNumber});
        }

        [HttpPost]
        public async Task<IActionResult> WithDraw(WithDrawVM withdrawViewModel)
        {
            ServiceResult result = await _accountService.Withdraw(withdrawViewModel);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            return RedirectToAction($"Details", new { id = withdrawViewModel.AccountNumber });
        }
        [HttpPost]
        public async Task<IActionResult> Transfer(TransferVM transferViewModel)
        {
            ServiceResult result = await _accountService.Transfer(transferViewModel);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.Message;
            Console.WriteLine(result.Message);
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
