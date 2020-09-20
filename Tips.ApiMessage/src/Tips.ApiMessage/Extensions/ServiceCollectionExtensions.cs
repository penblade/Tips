using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tips.ApiMessage.Configuration;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.CreateTodoItems;
using Tips.ApiMessage.TodoItems.DeleteTodoItems;
using Tips.ApiMessage.TodoItems.GetTodoItem;
using Tips.ApiMessage.TodoItems.GetTodoItems;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;
using Tips.ApiMessage.TodoItems.Rules.SaveRules;
using Tips.ApiMessage.TodoItems.Rules.UpdateRules;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

            var config = new AppConfiguration();
            configuration.Bind(nameof(AppConfiguration), config);
            services.AddSingleton(config);

            services.AddScoped(typeof(IPipelineBehavior), typeof(LoggingBehavior));

            services.AddScoped(typeof(IRulesFactory<Request<TodoItem>, Response<TodoItem>>), typeof(SaveRulesFactory));
            services.AddScoped(typeof(IRulesFactory<UpdateTodoItemRequest, Response<TodoItem>>), typeof(UpdateRulesFactory));
            services.AddScoped(typeof(IRulesEngine), typeof(RulesEngine));

            services.AddScoped(typeof(ICreateTodoItemRepository), typeof(CreateTodoItemRepository));
            services.AddScoped(typeof(IUpdateTodoItemRepository), typeof(UpdateTodoItemRepository));

            services.AddScoped(typeof(IRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>>), typeof(GetTodoItemsRequestHandler));
            services.AddScoped(typeof(IRequestHandler<GetTodoItemRequest, Response<TodoItem>>), typeof(GetTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>), typeof(CreateTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<DeleteTodoItemRequest, Response>), typeof(DeleteTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<UpdateTodoItemRequest, Response>), typeof(UpdateTodoItemRequestHandler));
        }
    }
}
