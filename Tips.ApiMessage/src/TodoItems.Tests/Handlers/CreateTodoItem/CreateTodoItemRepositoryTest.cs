using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.CreateTodoItem;
using TodoItems.Tests.Context;
using TodoItems.Tests.Support;

namespace TodoItems.Tests.Handlers.CreateTodoItem
{
    [TestClass]
    public class CreateTodoItemRepositoryTest : WithContext
    {
        [TestMethod]
        public async Task SaveAsyncTest()
        {
            // Arrange
            var todoItemEntity = TodoItemFactory.CreateTodoItemEntities(1).Single();
            var response = new Response<TodoItemEntity> { Item = todoItemEntity };

            // Act
            var repository = new CreateTodoItemRepository(Context);
            await repository.SaveAsync(response);

            // Assert
            var actualTodoItemEntity = await GetTodoItem(1);
            VerifyTodoItem.AssertTodoItem(response.Item, actualTodoItemEntity);
        }
    }
}
