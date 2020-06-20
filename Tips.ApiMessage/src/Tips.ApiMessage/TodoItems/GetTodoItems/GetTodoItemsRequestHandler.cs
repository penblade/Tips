using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;

namespace Tips.ApiMessage.TodoItems.GetTodoItems
{
    public class GetTodoItemsRequestHandler : IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse>
    {
        private readonly TodoContext _context;

        public GetTodoItemsRequestHandler(TodoContext context) => _context = context;

        public async Task<GetTodoItemsResponse> Handle(GetTodoItemsRequest request, CancellationToken cancellationToken)
        {
            var todoItems = await _context.TodoItems.Select(x => TodoItemMapper.ItemToResponse(x)).ToListAsync(cancellationToken);

            return new GetTodoItemsResponse
            {
                ApiMessage = new ApiMessage.Models.ApiMessage
                {
                    Status = (int) HttpStatusCode.OK,
                },
                TodoItems = todoItems
            };
        }
    }
}
