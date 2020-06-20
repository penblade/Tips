using Tips.ApiMessage.Contracts;

namespace Tips.ApiMessage.TodoItems.GetTodoItem
{
    public class GetTodoItemRequest : Request
    {
        public long Id { get; set; }
    }
}
