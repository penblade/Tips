using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Tips.DependencyInjectionOfInternals.Business.Configuration;
using Tips.DependencyInjectionOfInternals.Business.Models;
using Tips.DependencyInjectionOfInternals.Configuration;

namespace Tips.DependencyInjectionOfInternals
{
    public class Startup
    {
        // Inspiration
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1
        // https://stackoverflow.com/questions/49703773/implement-ado-connection-in-asp-net-core
        // https://medium.com/@mattmazzola/asp-net-core-injecting-custom-data-classes-into-startup-classs-constructor-and-configure-method-7cc146f00afb

        private readonly IServiceCollectionForBusiness _serviceCollectionForBusiness;

        public Startup(IServiceCollectionForBusiness serviceCollectionForBusiness)
        {
            _serviceCollectionForBusiness = serviceCollectionForBusiness;
        }

        // This method gets called by the runtime.
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AddMvc(services);
            AddBusinessLibrary(services);
        }

        private static void AddMvc(IServiceCollection services)
        {
            //var namespaceToTypes = typeof(ProcessRequest).Namespace;

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)

                // I prefer that Enums should be serialized
                // as their string names, not their integer
                // value.  As always, it depends on your app.
                // Adding the setting here saves us from
                // having references to Newtonsoft.Json
                // in the project libraries that don't care
                // about this "presentation" concern.
                .AddJsonOptions(options =>
                {
                    // Indented to make it easier to read during this demo.
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.IgnoreNullValues = false;

                    // [TODO] These are options are not supported in .NET Core 3.1 which uses the newer System.Text.Json.  I need to research how to handle this.

                    //options.JsonSerializerOptions.SerializationBinder = new CustomJsonSerializationBinder(namespaceToTypes);
                    //options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                });
        }

        // The IBusinessService is defined in the
        // same project as BusinessService, coupling the
        // Tips.DependencyInjectionOfInternals project to
        // the Tips.DependencyInjectionOfInternals.Business
        // project.

        // We could isolate the IBusinessService to it's
        // own project to support full abstraction allowing
        // for another project to be added that implements
        // IBusinessService and change the dependency
        // injection settings here to inject that instead.
        // However, since we would still have to couple
        // these two projects to add this registration,
        // we wouldn't have gained anything.

        // While Dependency Injection is great, don't
        // over complicate the dependencies when you know
        // there will only be one implementation ever.
        // With the pattern followed in this solution,
        // it's easy for us to refactor the interface
        // out later if we have a need.

        private void AddBusinessLibrary(IServiceCollection services)
        {
            _serviceCollectionForBusiness.RegisterDependencies(services);
        }

        // This method gets called by the runtime.
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
