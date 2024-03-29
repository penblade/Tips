﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.CreateTodoItem;
using Tips.TodoItems.Handlers.DeleteTodoItem;
using Tips.TodoItems.Handlers.GetTodoItem;
using Tips.TodoItems.Handlers.GetTodoItems;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Models;
using Tips.TodoItems.Rules.CreateRules;
using Tips.TodoItems.Rules.SaveRules;
using Tips.TodoItems.Rules.UpdateRules;

namespace Tips.TodoItems.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            // DbContext is added by default as scoped for APIs.
            // Any handlers, repositories, or other classes
            // consuming the database should also be scoped.
            // https://ardalis.com/avoid-wrapping-dbcontext-in-using/

            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

            // This is the order the rules will be run in.
            services.AddScoped(typeof(IBaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>), typeof(TodoItemIdRule));

            // This is the order the rules will be run in.
            services.AddScoped(typeof(IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>), typeof(TodoItemNotFoundRule));
            services.AddScoped(typeof(IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>), typeof(TodoItemNotSameIdRule));

            // This is the order the rules will be run in.
            services.AddScoped(typeof(IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(RequestRule));
            services.AddScoped(typeof(IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(ResponseRule));
            services.AddScoped(typeof(IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(TodoItemNameRule));
            services.AddScoped(typeof(IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(TodoItemDescriptionRule));
            services.AddScoped(typeof(IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(TodoItemPriorityRule));
            services.AddScoped(typeof(IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>), typeof(TodoItemReviewerRule));

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
