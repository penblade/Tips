using Tips.ApiMessage.Handlers;

namespace Tips.ApiMessage.Models
{
    public class TodoItemResponse : Response
    {
        public TodoItem TodoItem { get; set; }
    }
}
