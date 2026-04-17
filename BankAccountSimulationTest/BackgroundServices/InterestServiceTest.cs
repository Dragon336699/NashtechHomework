using BankSimulationMVC.BackgroundServices;
using BankSimulationMVC.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BankAccountSimulation.Test.BackgroundServices
{
    public class InterestServiceTest
    {
        [Fact]
        public async Task DoWorkOnce_ShouldCall_ProcessMonthlyInterest()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();

            var serviceProvider = new Mock<IServiceProvider>();
            var scope = new Mock<IServiceScope>();
            var scopeFactory = new Mock<IServiceScopeFactory>();

            scope.Setup(s => s.ServiceProvider).Returns(serviceProvider.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(scopeFactory.Object);

            scopeFactory
                .Setup(x => x.CreateScope())
                .Returns(scope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IAccountService)))
                .Returns(mockAccountService.Object);

            var service = new InterestService(serviceProvider.Object);

            // Act
            await service.DoWorkOnce();

            // Assert
            mockAccountService.Verify(x => x.ProcessMonthlyInterest(), Times.Once);
        }
    }
}
