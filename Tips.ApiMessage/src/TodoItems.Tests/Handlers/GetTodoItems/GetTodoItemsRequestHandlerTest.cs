using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.GetTodoItems;
using TodoItems.Tests.Context;

namespace TodoItems.Tests.Handlers.GetTodoItems
{
    [TestClass]
    public class GetTodoItemsRequestHandlerTest : WithContext
    {
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(25)]
        [DataRow(50)]
        [DataRow(200)]
        public async Task HandleAsyncTest(int totalItems)
        {
            var todoItemEntities = CreateTodoItemEntities(totalItems).ToList();
            await Context.AddRangeAsync(todoItemEntities);
            await Context.SaveChangesAsync();

            var handler = new GetTodoItemsRequestHandler(Context);

            var request = new GetTodoItemsRequest();
            var response = await handler.HandleAsync(request);

            Assert.AreEqual(todoItemEntities.Count, response.Item.Count);
            foreach (var todoItemEntity in todoItemEntities)
            {
                var todoItem = response.Item.SingleOrDefault(item => item.Id == todoItemEntity.Id);
                Assert.IsNotNull(todoItem);
                Assert.AreEqual(todoItemEntity.Id, todoItem.Id);
                Assert.AreEqual(todoItemEntity.Description, todoItem.Description);
                Assert.AreEqual(todoItemEntity.IsComplete, todoItem.IsComplete);
                Assert.AreEqual(todoItemEntity.Name, todoItem.Name);
                Assert.AreEqual(todoItemEntity.Priority, todoItem.Priority);
            }
        }

        private static IEnumerable<TodoItemEntity> CreateTodoItemEntities(int totalItems)
        {
            var todoItemEntities = new List<TodoItemEntity>();
            for (var i = 0; i < totalItems; i++)
            {
                todoItemEntities.Add(CreateTodoItemEntity(i + 1));
            }

            return todoItemEntities;
        }

        private static TodoItemEntity CreateTodoItemEntity(int id) =>
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
