using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Mappers
{
    internal class TodoItemMapper
    {
        public static TodoItem ItemToResponse(TodoItemEntity todoItemEntity) =>
            new TodoItem
            {
                Id = todoItemEntity.Id,
                Name = todoItemEntity.Name,
                Description = todoItemEntity.Description,
                Priority = todoItemEntity.Priority,
                IsComplete = todoItemEntity.IsComplete
            };
    }
}
