using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Mappers
{
    internal class TodoItemMapper
    {
        public static TodoItemEntity Map(TodoItem todoItem) => Map(todoItem, null);
        public static TodoItemEntity Map(TodoItem todoItem, TodoItemEntity todoItemEntity)
        {
            todoItemEntity ??= new TodoItemEntity();
            todoItemEntity.Id = todoItem?.Id ?? 0;
            todoItemEntity.Name = todoItem?.Name;
            todoItemEntity.Description = todoItem?.Description;
            todoItemEntity.Priority = todoItem?.Priority ?? 0;
            todoItemEntity.IsComplete = todoItem?.IsComplete ?? false;
            return todoItemEntity;
        }

        public static TodoItem Map(TodoItemEntity todoItemEntity) => Map(todoItemEntity, null);
        public static TodoItem Map(TodoItemEntity todoItemEntity, TodoItem todoItem)
        {
            todoItem ??= new TodoItem();
            todoItem.Id = todoItemEntity?.Id ?? 0;
            todoItem.Name = todoItemEntity?.Name;
            todoItem.Description = todoItemEntity?.Description;
            todoItem.Priority = todoItemEntity?.Priority ?? 0;
            todoItem.IsComplete = todoItemEntity?.IsComplete ?? false;
            return todoItem;
        }
    }
}
