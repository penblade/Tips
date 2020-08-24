using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    public class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
    {
        private readonly TodoContext _context;
        private readonly ITodoItemRulesEngine _todoItemRulesEngine;

        public CreateTodoItemRequestHandler(TodoContext context, ITodoItemRulesEngine todoItemRulesEngine)
        {
            _context = context;
            _todoItemRulesEngine = todoItemRulesEngine;
        }

        public async Task<Response<TodoItem>> Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItemEntity = new TodoItemEntity
            {
                IsComplete = request.TodoItem.IsComplete,
                Name = request.TodoItem.Name
            };

            var notifications = _todoItemRulesEngine.ProcessRules(request, todoItemEntity);

            if (notifications.Any(x => x.Severity == Notification.SeverityType.Error)) return BadRequest(notifications);

            await _context.TodoItems.AddAsync(todoItemEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Created(todoItemEntity);
        }

        private static Response<TodoItem> BadRequest(List<Notification> notifications) =>
            new Response<TodoItem> { Notifications = notifications ?? new List<Notification>(), Status = (int)HttpStatusCode.BadRequest };

        private static Response<TodoItem> Created(TodoItemEntity todoItemEntity) =>
            new Response<TodoItem>
            {
                Status = (int) HttpStatusCode.Created,
                Result = TodoItemMapper.ItemToResponse(todoItemEntity)
            };
    }
}
