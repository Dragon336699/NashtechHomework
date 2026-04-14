using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankSimulationMVC.Interfaces.Services;
using BankSimulationMVC.Domain.Entities;
using BankSimulationMVC.Application.Dtos.ViewModels;

namespace BankSimulationMVC.Controllers
{
    public class TransactionController : Controller
    {
        private int pageSize = 8;
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public async Task<IActionResult> Index(string type, int page = 1)
        {

            var query = _transactionService.Query();

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type.ToString().ToLower() == type);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var transactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Type = type;

            TransactionVM transactionVM = new TransactionVM
            {
                Transactions = transactions
            };

            return View(transactionVM);
        }

        public IActionResult History(string accountNumber)
        {
            if (!ModelState.IsValid)
            {
                return View(accountNumber);
            }

            IEnumerable<Transaction> transactions = _transactionService.GetTransactionsById(accountNumber);
            return View(transactions);
        }
    }
}
