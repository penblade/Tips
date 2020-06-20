using System.Collections.Generic;
using Tips.ApiMessage.Models;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.GetTodoItems
{
    public class GetTodoItemsResponse : Response
    {
        public IEnumerable<TodoItem> TodoItems { get; set; }
    }
}
