using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.CreateTodoItems;
using Tips.ApiMessage.TodoItems.DeleteTodoItems;
using Tips.ApiMessage.TodoItems.GetTodoItem;
using Tips.ApiMessage.TodoItems.GetTodoItems;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;
using System.Text.Json;

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
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            ConfigureDependencies(services);
            services.AddControllers();
        }

        private static void ConfigureDependencies(IServiceCollection services)
        {
            //services.AddScoped(typeof(IActionResultHandler<,>), typeof(ActionResultHandler<,>));
            services.AddScoped(typeof(IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse>), typeof(GetTodoItemsRequestHandler));
            services.AddScoped(typeof(IRequestHandler<GetTodoItemRequest, GetTodoItemResponse>), typeof(GetTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse>), typeof(CreateTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse>), typeof(DeleteTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<UpdateTodoItemRequest, UpdateTodoItemResponse>), typeof(UpdateTodoItemRequestHandler));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(errorApp =>
            {
                // https://www.strathweb.com/2018/07/centralized-exception-handling-and-request-validation-in-asp-net-core/
                errorApp.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    // ProblemDetails implements the RF7807 standards.
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Internal Server Error",
                        Status = (int)HttpStatusCode.InternalServerError,
                        Detail = "Internal Server Error",
                        // Generic organizational URI
                        Instance = $"urn:{CompanyConstants.UrnName}:error:{Guid.NewGuid()}" // change the guid to the TraceId.
                    };

                    // TODO: log the exception

                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions { WriteIndented = true }));
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
