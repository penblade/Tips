using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    public class CreateTodoItemResponse : Response
    {
        public TodoItem TodoItem { get; set; }
    }
}
