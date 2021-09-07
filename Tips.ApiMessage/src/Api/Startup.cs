using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tips.Api.Configuration;
using Tips.Middleware.Extensions;
using Tips.Security.Extensions;
using Tips.Swagger.Extensions;

namespace Tips.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterDependencies(_configuration);
            services.AddControllers();
            services.AddSwaggerWithApiKeySecurity(_configuration, $"{Assembly.GetExecutingAssembly().GetName().Name}");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();
            app.ConfigureExceptionHandler()
                .UseHttpsRedirection()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo Items v1");
                    // https://stackoverflow.com/questions/55914610/disable-try-it-out-in-swagger
                    //c.SupportedSubmitMethods();
                })
                .UseApiKeyHandlerMiddleware()
                .ConfigureHttpInfoLogger()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
