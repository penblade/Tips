using System.Collections.Generic;
using Tips.TodoItems.Context.Models;

namespace TodoItems.Tests.Support
{
    internal static class TodoItemFactory
    {
        public static IEnumerable<TodoItemEntity> CreateTodoItemEntities(int totalItems)
        {
            var todoItemEntities = new List<TodoItemEntity>();
            for (var i = 0; i < totalItems; i++)
            {
                todoItemEntities.Add(TodoItemFactory.CreateTodoItemEntity(i + 1));
            }

            return todoItemEntities;
        }

        public static TodoItemEntity CreateTodoItemEntity(int id) =>
            new()
            {
                Id = id,
                Description = $"TodoItem - Description - {id}",
                IsComplete = IsOdd(id),
                Name = $"TodoItem - Name - {id}",
                Priority = id + 1,
                Reviewer = $"TodoItem - Reviewer - {id}"
            };

        private static bool IsOdd(int id) => id % 2 == 1;
    }
}
