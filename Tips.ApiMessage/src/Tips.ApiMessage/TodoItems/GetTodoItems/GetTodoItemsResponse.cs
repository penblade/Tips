using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.GetTodoItems
{
    public class GetTodoItemsResponse : Response
    {
        public IEnumerable<TodoItem> TodoItems { get; set; }
    }
}
