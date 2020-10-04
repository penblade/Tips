using Tips.Pipeline;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Mappers
{
    internal class ResponseMapper
    {
        public static Response<TodoItem> MapToResponseWithTodoItem(Response<TodoItemEntity> response) =>
            new Response<TodoItem>
            {
                Item = TodoItemMapper.MapToTodoItem(response.Item),
                Notifications = response.Notifications
            };
    }
}
