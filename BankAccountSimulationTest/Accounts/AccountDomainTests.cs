using BankSimulationMVC.Domain.Entities;

namespace BankAccountSimulation.Test.Accounts
{
    public class AccountDomainTests
    {
        [Fact]
        public void UpdateBalance_ShouldUpdate_WhenBalanceIsValid()
        {
            // Arrange
            var account = new Account { AccountNumber = "1", OwnerName = "Long"};
            decimal newBalance = 1000;

            // Act
            account.UpdateBalance(newBalance);

            // Assert
            Assert.Equal(1000, account.Balance);
        }

        [Fact]
        public void UpdateBalance_ShouldUpdate_WhenBalanceIsNegative()
        {
            // Arrange
            var account = new Account { AccountNumber = "1", OwnerName = "Long" };
            decimal newBalance = -1000;

            // Act + Assert
            Assert.Throws<ArgumentException>(() => account.UpdateBalance(newBalance));
        }

    }
}
