using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.GetTodoItem
{
    public class GetTodoItemRequestHandler : IRequestHandler<GetTodoItemRequest, Response<TodoItem>>
    {
        private readonly TodoContext _context;

        public GetTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<Response<TodoItem>> Handle(GetTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync(request.Id);

            return todoItem != null ? Found(todoItem) : NotFound();
        }

        private static Response<TodoItem> Found(TodoItemEntity todoItemEntity) =>
            new Response<TodoItem>
            {
                Status = (int) HttpStatusCode.OK,
                Result = TodoItemMapper.ItemToResponse(todoItemEntity)
            };

        private static Response<TodoItem> NotFound() => new Response<TodoItem> { Status = (int) HttpStatusCode.NotFound };
    }
}
