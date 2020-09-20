using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.GetTodoItems
{
    public class GetTodoItemsRequestHandler : IRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>>
    {
        private readonly TodoContext _context;

        public GetTodoItemsRequestHandler(TodoContext context) => _context = context;

        public async Task<Response<List<TodoItem>>> Handle(GetTodoItemsRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<List<TodoItem>>();
            var todoItems = await _context.TodoItems.Select(todoItemEntity => TodoItemMapper.MapToTodoItem(todoItemEntity))
                .ToListAsync(cancellationToken);

            response.SetStatusToOk();
            response.Item = todoItems;
            return response;
        }
    }
}
