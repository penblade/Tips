using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.TodoItems.Handlers.GetTodoItems;
using Tips.TodoItems.Tests.Context;
using Tips.TodoItems.Tests.Support;

namespace Tips.TodoItems.Tests.Handlers.GetTodoItems
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
            var todoItemEntities = await PopulateTodoItems(totalItems);

            var handler = new GetTodoItemsRequestHandler(Context);

            var request = new GetTodoItemsRequest();
            var response = await handler.HandleAsync(request);

            Assert.IsNotNull(response?.Item);
            Assert.AreEqual(0, response.Notifications.Count);
            Assert.AreEqual(todoItemEntities.Count, response.Item.Count);

            foreach (var todoItemEntity in todoItemEntities)
            {
                var todoItem = response.Item.SingleOrDefault(item => item.Id == todoItemEntity.Id);
                VerifyTodoItem.AssertTodoItem(todoItemEntity, todoItem);
            }
        }
    }
}
