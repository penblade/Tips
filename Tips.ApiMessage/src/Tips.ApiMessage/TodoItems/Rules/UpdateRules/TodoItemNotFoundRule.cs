﻿using System.Linq;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.TodoItems.Rules.UpdateRules
{
    internal class TodoItemNotFoundRule : BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>
    {
        private readonly TodoContext _context;

        public TodoItemNotFoundRule(TodoContext context) => _context = context;

        protected override Task ProcessRule(UpdateTodoItemRequest request, Response<TodoItemEntity> response)
        {
            if (!TodoItemExists(request.Id))
            {
                response.Add(NotFoundNotification(request.Id));
                response.SetStatusToNotFound();
                ContinueProcessing = false;
                RuleFailed();
                return Task.CompletedTask;
            }

            RulePassed();
            return Task.CompletedTask;
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        internal const string NotFoundNotificationId = "9E1A675F-3073-4D78-9A22-317ECB1D88DC";
        private static Notification NotFoundNotification(long id) =>
            Notification.CreateError(NotFoundNotificationId, $"TodoItem {id} was not found.");
    }
}
