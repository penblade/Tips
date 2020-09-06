using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    public class UpdateTodoItemRequestHandler : IRequestHandler<UpdateTodoItemRequest, Response>
    {
        private readonly TodoContext _context;
        private readonly ITodoItemRulesEngine _todoItemRulesEngine;

        public UpdateTodoItemRequestHandler(TodoContext context, ITodoItemRulesEngine todoItemRulesEngine)
        {
            _context = context;
            _todoItemRulesEngine = todoItemRulesEngine;
        }

        public async Task<Response> Handle(UpdateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<TodoItem>();

            if (request?.TodoItem == null)
            {
                response.Add(TodoItemWasNotProvidedNotification());
                response.SetStatusToBadRequest();
                return response;
            }

            if (request.Id != request.TodoItem.Id)
            {
                response.Add(NotSameIdNotification(request.Id, request.TodoItem.Id));
                response.SetStatusToBadRequest();
                return response;
            }

            var todoItemEntity = await _context.TodoItems.FindAsync(request.Id);
            if (todoItemEntity == null)
            {
                response.Add(NotFoundNotification(request.Id));
                response.SetStatusToNotFound();
                return response;
            }

            _todoItemRulesEngine.ProcessRules(request, response);

            if (response.HasErrors())
            {
                response.SetStatusToBadRequest();
                return response;
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(request.Id))
            {
                response.Add(NotFoundWhenSavingNotification(request.Id));
                response.SetStatusToNotFound();
                return response;
            }

            response.SetStatusToNoContent();
            return response;
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        internal const string NotSameIdNotificationId = "38EFC3AD-7A84-4D49-85F5-E325125A6EE1";
        private static Notification NotSameIdNotification(long requestId, long todoItemId) =>
            Notification.CreateError(NotSameIdNotificationId, $"TodoItem {requestId} does not match {todoItemId}.");

        internal const string NotFoundNotificationId = "9E1A675F-3073-4D78-9A22-317ECB1D88DC";
        private static Notification NotFoundNotification(long id) =>
            Notification.CreateError(NotFoundNotificationId, $"TodoItem {id} was not found.");

        internal const string NotFoundWhenSavingNotificationId = "8FD46D5D-1CB3-4ECB-B27B-724813A0406C";
        private static Notification NotFoundWhenSavingNotification(long id) =>
            Notification.CreateError(NotFoundWhenSavingNotificationId, $"TodoItem {id} was not found when saving.");

        internal const string TodoItemWasNotProvidedNotificationId = "DC02BFB8-F28D-4CA7-8EFB-74A4E89C1558";
        private static Notification TodoItemWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemWasNotProvidedNotificationId, "TodoItem was not provided.");
    }
}
