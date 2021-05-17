using System.Collections.Generic;
using Extensions;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace TodoItems.Tests.Support
{
    internal static class TodoItemFactory
    {
        public static IEnumerable<TodoItemEntity> CreateTodoItemEntities(int totalItems)
        {
            var todoItemEntities = new List<TodoItemEntity>();
            for (var i = 0; i < totalItems; i++)
            {
                todoItemEntities.Add(CreateTodoItemEntity(i + 1));
            }

            return todoItemEntities;
        }

        public static TodoItemEntity CreateTodoItemEntity(int id) =>
            new()
            {
                Id = id,
                Description = BuildStringProperty(nameof(TodoItemEntity.Description), id),
                IsComplete = IsOdd(id),
                Name = BuildStringProperty(nameof(TodoItemEntity.Name), id),
                Priority = id + 1,
                Reviewer = BuildStringProperty(nameof(TodoItemEntity.Reviewer), id)
            };

        public static TodoItem CreateTodoItem(int id) => CreateTodoItemEntity(id).Clone<TodoItemEntity, TodoItem>();

        private static string BuildStringProperty(string name, int id) => $"TodoItem - {name} - {id}";

        private static bool IsOdd(int id) => id % 2 == 1;
    }
}
