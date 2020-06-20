using Tips.ApiMessage.Models;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.GetTodoItem
{
    public class GetTodoItemResponse : Response
    {
        public TodoItem TodoItem { get; set; }
    }
}
