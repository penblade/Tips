using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class GetTodoItemsRequestHandler : IRequestHandler<GetTodoItemsRequest, Response<IEnumerable<TodoItem>>>
    {
        private readonly TodoContext _context;

        public GetTodoItemsRequestHandler(TodoContext context) => _context = context;

        public async Task<Response<IEnumerable<TodoItem>>> Handle(GetTodoItemsRequest request, CancellationToken cancellationToken)
        {
            var todoItems = await _context.TodoItems.Select(x => TodoItemMapper.Map(x)).ToListAsync(cancellationToken);

            return Ok(todoItems);
        }

        private static Response<IEnumerable<TodoItem>> Ok(IEnumerable<TodoItem> todoItems) =>
            new Response<IEnumerable<TodoItem>>
            {
                Status = (int) HttpStatusCode.OK,
                Result = todoItems
            };
    }
}
