using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.DependencyInjectionOfInternals.Business.Configuration;

namespace Tips.DependencyInjectionOfInternals.Tests
{
    [TestClass]
    public class StartupTest
    {
        // Requires AspNetCore dependency for the
        // ServiceCollection to call the static
        // extension method AddMvc().
        [TestMethod]
        public void VerifyRegisterDependenciesForBusinessWasRegistered()
        {
            // Requires AspNetCore dependency for the
            // ServiceCollection to call the static
            // extension method AddMvc().
            var serviceCollection = new ServiceCollection();

            var mockServiceCollectionForBusiness = new Mock<IServiceCollectionForBusiness>();
            mockServiceCollectionForBusiness.Setup(x => x.RegisterDependencies(serviceCollection));

            var startup = new Startup(mockServiceCollectionForBusiness.Object);

            startup.ConfigureServices(serviceCollection);

            // Verify that the static method was called once.
            mockServiceCollectionForBusiness.Verify(x => x.RegisterDependencies(serviceCollection), Times.Once);
        }
    }
}
