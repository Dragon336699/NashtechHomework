using BankAccountSimulation;
using BankAccountSimulation.Enum;
using System.Text.Json;

namespace BankAccountApp
{
    class Program
    {
        static string bankAccountFileName = "bankAccounts.txt";
        static string transactionFileName = "transactions.txt";
        static List<BankAccount> bankAccounts = new List<BankAccount>();
        static List<Transaction> transactions = new List<Transaction>();
        static void Main(string[] args)
        {
            ReadFile();
            while (true)
            {
                Console.WriteLine("=== Bank Account System ===");
                Console.WriteLine("1. Create new account");
                Console.WriteLine("2. Deposit money");
                Console.WriteLine("3. Withdraw money");
                Console.WriteLine("4. Transfer money");
                Console.WriteLine("5. View account details");
                Console.WriteLine("6. View transaction history");
                Console.WriteLine("7. Freeze / Unfreeze account");
                Console.WriteLine("8. Exit");
                Notify("Select your option: ", MessageStatus.Question);
                int.TryParse(Console.ReadLine(), out int enteredOption);
                switch (enteredOption)
                {
                    case 1:
                        CreateNewAccount();
                        break;
                    case 2:
                        DepositMoney();
                        break;
                    case 3:
                        WithdrawMoney();
                        break;
                    case 4:
                        TransferMoney();
                        break;
                    case 5:
                        ViewAccountDetails();
                        break;
                    case 6:
                        ViewTransactionHistory();
                        break;
                    case 7:
                        ChangeAccountStatus();
                        break;
                    case 8:
                        return;
                    default:
                        Notify("You enter the option isn't in the list. Please try again!", MessageStatus.Fail);
                        break;
                }
            }
        }

        public static void CreateNewAccount()
        {
            try
            {
                Notify("Enter new account number: ", MessageStatus.Question);
                string accountNumber = Console.ReadLine();
                if (string.IsNullOrEmpty(accountNumber))
                {
                    Notify("Account number cannot be empty. Please try again!", MessageStatus.Fail);
                    return;
                }

                BankAccount? existBankAccount = bankAccounts.Find(ba => ba.AccountNumber == accountNumber);
                if (existBankAccount != null)
                {
                    Notify("Account number existed. Please try again!", MessageStatus.Fail);
                    return;
                }

                Notify("Enter owner name: ", MessageStatus.Question);
                string ownerName = Console.ReadLine();
                if (string.IsNullOrEmpty(ownerName))
                {
                    Notify("Owner name cannot be empty", MessageStatus.Fail);
                    return;
                }

                Notify("Enter initial balance:  ", MessageStatus.Question);

                if (!int.TryParse(Console.ReadLine(), out int initialBalance))
                {
                    Notify("Format of balance false. Please try again!", MessageStatus.Fail);
                    return;
                }

                if (initialBalance < 0)
                {
                    Notify("Initial balance cannot be lower than 0", MessageStatus.Fail);
                    return;
                }

                BankAccount newBankAccount = new BankAccount(accountNumber, ownerName, initialBalance);
                bankAccounts.Add(newBankAccount);
                WriteToFile(bankAccountFileName, bankAccounts);
                Notify("Account created successfully", MessageStatus.Success);
            }
            catch (Exception ex)
            {
                Notify(ex.Message, MessageStatus.Fail);
            }
        }

        public static void DepositMoney()
        {
            Notify("Enter your account number: ", MessageStatus.Question);
            try
            {
                string accountNumber = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    Notify("Account number cannot be empty. Please try again!", MessageStatus.Fail);
                    return;
                }
                BankAccount? existBankAccount = bankAccounts.Find(ba => ba.AccountNumber == accountNumber);
                if (existBankAccount == null)
                {
                    Notify("Account number not existed. Please try again!", MessageStatus.Fail);
                    return;
                }

                Notify("Enter amount you want to deposit: ", MessageStatus.Question);

                if (!int.TryParse(Console.ReadLine(), out int amount))
                {
                    Notify("Format of amount false. Please try again", MessageStatus.Fail);
                    return;
                }

                existBankAccount.DepositMoney(amount);

                AddTransaction(accountNumber, TransactionType.Deposit, amount);

                WriteToFile(bankAccountFileName, bankAccounts);

                WriteToFile(transactionFileName, transactions);

                Console.WriteLine($"Your new balance is: {existBankAccount.Balance}");
            }
            catch (Exception ex)
            {
                Notify(ex.Message, MessageStatus.Fail);
            }
        }

        public static void WithdrawMoney()
        {
            Notify("Enter your account number: ", MessageStatus.Question);
            try
            {
                string accountNumber = Console.ReadLine();
                if (string.IsNullOrEmpty(accountNumber))
                {
                    Notify("Account number cannot be empty. Please try again!", MessageStatus.Fail);
                    return;
                }

                BankAccount? existBankAccount = bankAccounts.Find(ba => ba.AccountNumber == accountNumber);
                if (existBankAccount == null)
                {
                    Notify("Account number not existed. Please try again!", MessageStatus.Fail);
                    return;
                }

                Notify("Enter amount you want to withdraw: ", MessageStatus.Question);

                if (!int.TryParse(Console.ReadLine(), out int amount))
                {
                    Notify("Format of amount false. Please try again", MessageStatus.Fail);
                    return;
                }

                existBankAccount.WithDrawMoney(amount);

                AddTransaction(accountNumber, TransactionType.WithDraw, amount);

                WriteToFile(bankAccountFileName, bankAccounts);

                WriteToFile(transactionFileName, transactions);

                Console.WriteLine($"Remaining balance is: {existBankAccount.Balance}");
            }
            catch (Exception ex)
            {
                Notify(ex.Message, MessageStatus.Fail);
            }
        }

        public static void TransferMoney()
        {
            try
            {
                Notify("Transfer from account: ", MessageStatus.Question);
                string accountNumber = Console.ReadLine();

                if (string.IsNullOrEmpty(accountNumber))
                {
                    Notify("Account number cannot be empty. Please try again!", MessageStatus.Fail);
                    return;
                }

                BankAccount? sourceBankAccount = bankAccounts.Find(ba => ba.AccountNumber == accountNumber);
                if (sourceBankAccount == null)
                {
                    Notify("Account not existed. Please try again!", MessageStatus.Fail);
                    return;
                }

                Notify("Transfer to account: ", MessageStatus.Question);
                string desAccountNumber = Console.ReadLine();

                if (string.IsNullOrEmpty(desAccountNumber))
                {
                    Notify("Account number cannot be empty. Please try again!", MessageStatus.Fail);
                    return;
                }

                BankAccount? existDesBankAccount = bankAccounts.Find(ba => ba.AccountNumber == desAccountNumber);
                if (existDesBankAccount == null)
                {
                    Notify("Destination account not existed. Please try again!", MessageStatus.Fail);
                    return;
                }

                Notify("Enter amount you want to transfer: ", MessageStatus.Question);

                if (!int.TryParse(Console.ReadLine(), out int amount))
                {
                    Notify("Format of amount false. Please try again", MessageStatus.Fail);
                    return;
                }

                sourceBankAccount.WithDrawMoney(amount);

                try
                {
                    existDesBankAccount.DepositMoney(amount);
                }
                catch (Exception ex)
                {
                    sourceBankAccount.DepositMoney(amount);
                    sourceBankAccount.WithdrawnToday -= amount;
                    Notify(ex.Message, MessageStatus.Fail);
                    return;
                }


                AddTransaction(accountNumber, TransactionType.TransferOut, amount);

                AddTransaction(desAccountNumber, TransactionType.TransferIn, amount);

                WriteToFile(bankAccountFileName, bankAccounts);

                WriteToFile(transactionFileName, transactions);

                Notify("Transfer completed successfully", MessageStatus.Success);
            }
            catch (Exception ex)
            {
                Notify(ex.Message, MessageStatus.Fail);
            }
        }

        public static void ViewAccountDetails()
        {
            try
            {
                if (bankAccounts.Count == 0)
                {
                    Notify("No bank account found.", MessageStatus.Fail);
                }
                else
                {
                    foreach (var account in bankAccounts)
                    {
                        Console.WriteLine(account.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                Notify(ex.Message, MessageStatus.Fail);
            }
        }

        public static void ViewTransactionHistory()
        {
            Notify("Enter account number you want to view transaction history: ", MessageStatus.Question);
            try
            {
                string accountNumber = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    Notify("Account number cannot be empty. Please try again!", MessageStatus.Fail);
                    return;
                }

                var filteredTransaction = transactions.Where(t => t.AccountNumber == accountNumber).ToList();

                Console.WriteLine("1. All transactions");
                Console.WriteLine("2. Deposits only");
                Console.WriteLine("3. Withdrawals only");
                Notify("Filter transactions you want to view: ", MessageStatus.Question);

                if (!int.TryParse(Console.ReadLine(), out int enteredOption))
                {
                    Notify("Option must be numeric. Please try again!", MessageStatus.Fail);
                    return;
                }



                switch (enteredOption)
                {
                    case 1:
                        foreach (var transaction in filteredTransaction)
                        {
                            Console.WriteLine(transaction.ToString());
                        }
                        break;
                    case 2:
                        var depositsTransaction = filteredTransaction.Where(t => t.Type == TransactionType.Deposit).ToList();
                        foreach (var transaction in depositsTransaction)
                        {
                            Console.WriteLine(transaction.ToString());
                        }
                        break;
                    case 3:
                        var withdrawalsTransactions = filteredTransaction.Where(t => t.Type == TransactionType.WithDraw).ToList();
                        foreach (var transaction in withdrawalsTransactions)
                        {
                            Console.WriteLine(transaction.ToString());
                        }
                        break;
                    default:
                        Notify("Your option is not in the list. Please try again.", MessageStatus.Fail);
                        break;
                }
            }
            catch (Exception ex)
            {
                Notify(ex.Message, MessageStatus.Fail);
            }
        }

        public static void ChangeAccountStatus()
        {
            Notify("Enter your account number you want to change status: ", MessageStatus.Question);
            try
            {
                string enteredAccountNumber = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(enteredAccountNumber))
                {
                    Notify("Account number cannot be empty. Please try again!", MessageStatus.Fail);
                    return;
                }

                BankAccount? bankAccount = bankAccounts.Find(ba => ba.AccountNumber == enteredAccountNumber);

                if (bankAccount == null)
                {
                    Notify("Cannot find the bank account.", MessageStatus.Fail);
                    return;
                }
                Console.WriteLine("1. Freeze");
                Console.WriteLine("2. Unfreeze");
                Notify("Choose status you want to change: ", MessageStatus.Question);

                if (!int.TryParse(Console.ReadLine(), out int enteredOption))
                {
                    Notify("Option must be numeric. Please try again!", MessageStatus.Fail);
                    return;
                }

                if (enteredOption == 1)
                {
                    if (bankAccount.Status == BankAccountStatus.Frozen)
                    {
                        Notify("Your account already frozen.", MessageStatus.Fail);
                    }
                    else
                    {
                        bankAccount.Status = BankAccountStatus.Frozen;
                        Notify("Freeze account successfully!", MessageStatus.Success);
                    }
                }
                else if (enteredOption == 2)
                {
                    if (bankAccount.Status == BankAccountStatus.Active)
                    {
                        Notify("Your account already activated.", MessageStatus.Fail);
                    }
                    else
                    {
                        bankAccount.Status = BankAccountStatus.Active;
                        Notify("Active account successfully!", MessageStatus.Success);
                    }
                }
                else
                {
                    Notify("Your option you chose is not in the list. Please try again!", MessageStatus.Fail);
                    return;
                }


                WriteToFile(bankAccountFileName, bankAccounts);

                WriteToFile(transactionFileName, transactions);
            }
            catch (Exception ex)
            {
                Notify(ex.Message, MessageStatus.Fail);
            }
        }

        public static void AddTransaction(string accountNumber, TransactionType transactionType, decimal amount)
        {
            Transaction newTransaction = new Transaction
            {
                AccountNumber = accountNumber,
                TransactionId = transactions.Count + 1,
                Type = transactionType,
                Amount = amount,
                Date = DateTime.Now
            };

            transactions.Add(newTransaction);
        }

        public static void ReadFile()
        {
            if (!File.Exists(bankAccountFileName))
            {
                File.Create(bankAccountFileName);
            }

            if (!File.Exists(transactionFileName))
            {
                File.Create(transactionFileName);
            }

            var bankAccountsText = File.ReadAllText(bankAccountFileName);
            if (!string.IsNullOrWhiteSpace(bankAccountsText))
            {
                bankAccounts = JsonSerializer.Deserialize<List<BankAccount>>(bankAccountsText);
            }
            var transactionText = File.ReadAllText(transactionFileName);
            if (!string.IsNullOrWhiteSpace(transactionText))
            {
                transactions = JsonSerializer.Deserialize<List<Transaction>>(transactionText);
            }
        }

        public static void WriteToFile<T>(string fileName, List<T> objects)
        {
            string json = JsonSerializer.Serialize(objects);
            File.WriteAllText(fileName, json);
        }

        public static void Notify(string message, MessageStatus status)
        {
            switch (status)
            {
                case MessageStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(message);
                    Console.ResetColor();
                    break;
                case MessageStatus.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(message);
                    Console.ResetColor();
                    break;
                case MessageStatus.Question:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(message);
                    Console.ResetColor();
                    break;
                case MessageStatus.Fail:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                    Console.ResetColor();
                    break;
                default:
                    break;
            }
        }
    }
}
