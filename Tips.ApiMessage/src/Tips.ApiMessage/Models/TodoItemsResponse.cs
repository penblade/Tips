using System.Collections;
using System.Collections.Generic;
using Tips.ApiMessage.Handlers;

namespace Tips.ApiMessage.Models
{
    public class TodoItemsResponse : Response
    {
        public IEnumerable<TodoItem> TodoItems { get; set; }
    }
}
