using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    public class UpdateTodoItemRequestHandler : IRequestHandler<UpdateTodoItemRequest, UpdateTodoItemResponse>
    {
        private readonly TodoContext _context;

        public UpdateTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<UpdateTodoItemResponse> Handle(UpdateTodoItemRequest request, CancellationToken cancellationToken)
        {
            if (request.Id != request.TodoItem.Id) return BadRequest(new List<Notification> {CreateNotSameIdNotification(request)});

            var todoItemEntity = await _context.TodoItems.FindAsync(request.Id);
            if (todoItemEntity == null) return NotFound();

            todoItemEntity.Name = request.TodoItem.Name;
            todoItemEntity.IsComplete = request.TodoItem.IsComplete;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(request.Id))
            {
                return NotFound(new List<Notification> {CreateNotFoundNotification(request)});
            }

            // I prefer to return an Ok response with a message that includes any possible notifications.
            // I find it's easier for the client to have a standard response to implement.
            // return NoContent();
            return Ok();
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        private static UpdateTodoItemResponse BadRequest(IEnumerable<Notification> notifications = null) =>
            new UpdateTodoItemResponse
            {
                Notifications = notifications,
                Status = (int)HttpStatusCode.BadRequest
                //TraceId = TraceId
            };

        private static UpdateTodoItemResponse Ok(IEnumerable<Notification> notifications = null) =>
            new UpdateTodoItemResponse
            {
                Notifications = notifications,
                Status = (int) HttpStatusCode.OK
                //TraceId = TraceId
            };

        private static UpdateTodoItemResponse NotFound(IEnumerable<Notification> notifications = null) =>
            new UpdateTodoItemResponse
            {
                Notifications = notifications,
                Status = (int)HttpStatusCode.NotFound
                //TraceId = TraceId
            };

        private static Notification CreateNotSameIdNotification(UpdateTodoItemRequest request) =>
            new NotificationBuilder()
                .Id("38EFC3AD-7A84-4D49-85F5-E325125A6EE1")
                .Severity(Severity.Error)
                .Detail($"TodoItem {request.Id} does not match {request.TodoItem.Id}.")
                .Build();

        private static Notification CreateNotFoundNotification(UpdateTodoItemRequest request) =>
            new NotificationBuilder()
                .Id("9E1A675F-3073-4D78-9A22-317ECB1D88DC")
                .Severity(Severity.Error)
                .Detail($"TodoItem {request.Id} was not found.")
                .Build();
    }
}
