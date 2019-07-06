using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tips.DependencyInjectionOfInternals.Business.Configuration;

namespace Tips.DependencyInjectionOfInternals
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // Required to create configurations from JSON files.
                .ConfigureServices(services => services.AddTransient<IConfigurationBuilder, ConfigurationBuilder>())

                // Add custom service collection registration.

                // In order to inject classes into the
                // Startup constructor, the services must
                // be registered before we UseStartup.

                // ConfigureServices must be called before
                // UseStartup method.  This is how .NET Core
                // works under the covers as noted in:
                // Asp.Net Core: Injecting custom 
                // data/classes into startup classes’
                // constructor and configure method
                // by Matt Mazzola.
                // https://medium.com/@mattmazzola/asp-net-core-injecting-custom-data-classes-into-startup-classs-constructor-and-configure-method-7cc146f00afb

                .ConfigureServices(services => services.AddTransient<IServiceCollectionForBusiness, ServiceCollectionForBusiness>())
                .UseStartup<Startup>();
    }
}
