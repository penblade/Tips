using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.CreateTodoItems;
using Tips.TodoItems.Handlers.DeleteTodoItems;
using Tips.TodoItems.Handlers.GetTodoItem;
using Tips.TodoItems.Handlers.GetTodoItems;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Models;
using Tips.TodoItems.Rules.SaveRules;
using Tips.TodoItems.Rules.UpdateRules;

namespace Tips.ApiMessage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

            var config = new ExceptionHandlerMiddlewareConfiguration();
            configuration.Bind(nameof(ExceptionHandlerMiddlewareConfiguration), config);
            services.AddSingleton(config);

            services.AddScoped(typeof(IPipelineBehavior), typeof(LoggingBehavior));

            services.AddScoped(typeof(IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>), typeof(SaveRulesFactory));
            services.AddScoped(typeof(IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>>), typeof(UpdateRulesFactory));
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
