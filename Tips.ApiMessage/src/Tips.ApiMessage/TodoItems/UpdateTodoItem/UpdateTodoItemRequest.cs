﻿using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Endpoint.Models;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    public class UpdateTodoItemRequest : Request<TodoItem>
    {
        public long Id { get; set; }
    }
}
