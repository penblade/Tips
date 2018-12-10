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

            var mockConfiguration = new Mock<IConfiguration>();

            var mockServiceCollectionForBusiness = new Mock<IServiceCollectionForBusiness>();
            mockServiceCollectionForBusiness.Setup(x => x.RegisterDependencies(mockConfiguration.Object, serviceCollection));

            var startup = new Startup(mockConfiguration.Object, mockServiceCollectionForBusiness.Object);

            startup.ConfigureServices(serviceCollection);

            // Verify that the static method was called once.
            mockServiceCollectionForBusiness.Verify(x => x.RegisterDependencies(mockConfiguration.Object, serviceCollection), Times.Once);
        }
    }
}
