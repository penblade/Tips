using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Context;
using Tips.ApiMessage.Models;

namespace Tips.ApiMessage.Handlers
{
    public class GetTodoItemsRequestHandler : IRequestHandler<TodoItemsQuery, TodoItemsResponse>
    {
        private readonly TodoContext _context;

        public GetTodoItemsRequestHandler(TodoContext context) => _context = context;

        public async Task<TodoItemsResponse> Handle(TodoItemsQuery request, CancellationToken cancellationToken)
        {
            var todoItems = await _context.TodoItems.Select(x => TodoItemMapper.ItemToResponse(x)).ToListAsync(cancellationToken);

            return new TodoItemsResponse
            {
                ApiMessage = new Messages.ApiMessage
                {
                    Status = (int) HttpStatusCode.OK,
                },
                TodoItems = todoItems
            };
        }
    }
}
