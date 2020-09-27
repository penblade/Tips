﻿using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Mappers;

namespace Tips.ApiMessage.TodoItems.GetTodoItem
{
    public class GetTodoItemRequestHandler : IRequestHandler<GetTodoItemRequest, Response<TodoItem>>
    {
        private readonly TodoContext _context;

        public GetTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<Response<TodoItem>> HandleAsync(GetTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<TodoItem>();
            var todoItemEntity = await _context.TodoItems.FindAsync(request.Id);

            if (todoItemEntity != null)
            {
                response.SetStatusToOk();
                response.Item = TodoItemMapper.MapToTodoItem(todoItemEntity);
                return response;
            }

            response.SetStatusToNotFound();
            return response;
        }
    }
}
