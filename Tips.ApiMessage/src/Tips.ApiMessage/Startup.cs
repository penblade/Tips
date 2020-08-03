using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.CreateTodoItems;
using Tips.ApiMessage.TodoItems.DeleteTodoItems;
using Tips.ApiMessage.TodoItems.GetTodoItem;
using Tips.ApiMessage.TodoItems.GetTodoItems;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDependenciesForTodoItems(services);
            services.AddControllers();
        }

        private static void ConfigureDependenciesForTodoItems(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

            services.AddScoped(typeof(IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse>), typeof(GetTodoItemsRequestHandler));
            services.AddScoped(typeof(IRequestHandler<GetTodoItemRequest, GetTodoItemResponse>), typeof(GetTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse>), typeof(CreateTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse>), typeof(DeleteTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<UpdateTodoItemRequest, UpdateTodoItemResponse>), typeof(UpdateTodoItemRequestHandler));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            app.UseExceptionHandler(errorHandler =>
            {
                errorHandler.Run(async context =>
                {
                    // TODO: Logger is still logging the exception without scope.

                    const string uncaughtExceptionId = "D1537B75-D85A-48CF-8A02-DF6C614C3198";
                    //using var scope = _logger.BeginScope(nameof(Error));

                    //var feature = context.Features.Get<IExceptionHandlerFeature>();
                    //var exception = feature.Error;

                    // ProblemDetails implements the RF7807 standards.
                    // TraceId is already returned.
                    var problemDetails = new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        Title = "Internal Server Error",
                        Status = (int)HttpStatusCode.InternalServerError,
                        Detail = "Internal Server Error",
                        Instance = $"urn:{CompanyConstants.UrnName}:error:{uncaughtExceptionId}",
                    };

                    problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

                    //_logger.LogError(exception, "Uncaught Exception", exception);

                    context.Response.StatusCode = (int)problemDetails.Status;
                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
