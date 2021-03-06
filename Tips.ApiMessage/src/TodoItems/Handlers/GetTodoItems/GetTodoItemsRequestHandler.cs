﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.Pipeline;
using Tips.TodoItems.Context;
using Tips.TodoItems.Mappers;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Handlers.GetTodoItems
{
    internal class GetTodoItemsRequestHandler : IRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>>
    {
        private readonly TodoContext _context;

        public GetTodoItemsRequestHandler(TodoContext context) => _context = context;

        public async Task<Response<List<TodoItem>>> HandleAsync(GetTodoItemsRequest request)
        {
            var todoItems = await _context.TodoItems.Select(todoItemEntity => TodoItemMapper.MapToTodoItem(todoItemEntity))
                .ToListAsync();

            return new Response<List<TodoItem>>(todoItems);
        }
    }
}
