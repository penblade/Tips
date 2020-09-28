using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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

namespace Tips.TodoItems.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

            services.AddScoped(typeof(IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>), typeof(SaveRulesFactory));
            services.AddScoped(typeof(IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>>), typeof(UpdateRulesFactory));

            services.AddScoped(typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(RequestRule));
            services.AddScoped(typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(ResponseRule));
            services.AddScoped(typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(TodoItemNameRule));
            services.AddScoped(typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(TodoItemDescriptionRule));
            services.AddScoped(typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(TodoItemPriorityRule));
            services.AddScoped(typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(TodoItemReviewerRule));

            services.AddScoped(typeof(BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>), typeof(TodoItemNotSameIdRule));
            services.AddScoped(typeof(BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>), typeof(TodoItemNotFoundRule));

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
