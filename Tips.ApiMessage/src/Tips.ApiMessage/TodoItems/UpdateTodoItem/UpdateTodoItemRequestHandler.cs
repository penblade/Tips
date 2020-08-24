using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
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
            if (request?.TodoItem == null) return BadRequest(TodoItemWasNotProvidedNotification());
            if (request.Id != request.TodoItem.Id) return BadRequest(NotSameIdNotification(request));

            var todoItemEntity = await _context.TodoItems.FindAsync(request.Id);
            if (todoItemEntity == null) return NotFound(NotFoundNotification(request));

            var notifications = _todoItemRulesEngine.ProcessRules(request, todoItemEntity);

            if (notifications.Any(x => x.Severity == Severity.Error)) return BadRequest(notifications);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(request.Id))
            {
                return NotFound(NotFoundWhenSavingNotification(request));
            }

            return NoContent();
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        private static Response BadRequest(Notification notification) => BadRequest(new List<Notification> {notification});

        private static Response BadRequest(List<Notification> notifications) =>
            new Response { Notifications = notifications ?? new List<Notification>(), Status = (int)HttpStatusCode.BadRequest };

        private static Response NotFound(Notification notification) =>
            new Response { Notifications = new List<Notification> { notification }, Status = (int)HttpStatusCode.NotFound };

        private static Response NoContent() => new Response { Status = (int)HttpStatusCode.NoContent };

        private static Notification NotSameIdNotification(UpdateTodoItemRequest request) =>
            new NotificationBuilder()
                .Id("38EFC3AD-7A84-4D49-85F5-E325125A6EE1")
                .Severity(Severity.Error)
                .Detail($"TodoItem {request.Id} does not match {request.TodoItem.Id}.")
                .Build();

        private static Notification NotFoundNotification(UpdateTodoItemRequest request) =>
            new NotificationBuilder()
                .Id("9E1A675F-3073-4D78-9A22-317ECB1D88DC")
                .Severity(Severity.Error)
                .Detail($"TodoItem {request.Id} was not found.")
                .Build();

        private static Notification NotFoundWhenSavingNotification(UpdateTodoItemRequest request) =>
            new NotificationBuilder()
                .Id("8FD46D5D-1CB3-4ECB-B27B-724813A0406C")
                .Severity(Severity.Error)
                .Detail($"TodoItem {request.Id} was not found when saving.")
                .Build();

        private static Notification TodoItemWasNotProvidedNotification() =>
            new NotificationBuilder()
                .Id("DC02BFB8-F28D-4CA7-8EFB-74A4E89C1558")
                .Severity(Severity.Error)
                .Detail("TodoItem was not provided.")
                .Build();
    }
}
