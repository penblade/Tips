using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.TodoItems.Handlers.GetTodoItem;
using Tips.TodoItems.Tests.Context;
using Tips.TodoItems.Tests.Support;

namespace Tips.TodoItems.Tests.Handlers.GetTodoItem
{
    [TestClass]
    public class GetTodoItemRequestHandlerTest : WithContext
    {
        [TestMethod]
        [DataRow(2, 1)]
        [DataRow(2, 2)]
        public async Task HandleAsyncTest(int totalItems, int requestId)
        {
            var todoItemEntities = await PopulateTodoItems(totalItems);
            var expectedTodoItemEntity = todoItemEntities.SingleOrDefault(item => item.Id == requestId);

            var handler = new GetTodoItemRequestHandler(Context);

            var request = new GetTodoItemRequest { Id = requestId };
            var response = await handler.HandleAsync(request);

            Assert.IsNotNull(response);
            Assert.AreEqual(0, response.Notifications.Count);
            VerifyTodoItem.AssertTodoItem(expectedTodoItemEntity, response.Item);
        }

        [TestMethod]
        [DataRow(2, 0)]
        [DataRow(2, 3)]
        public async Task HandleAsyncNotFoundTest(int totalItems, int requestId)
        {
            var todoItemEntities = await PopulateTodoItems(totalItems);
            var expectedTodoItemEntity = todoItemEntities.SingleOrDefault(item => item.Id == requestId);

            var handler = new GetTodoItemRequestHandler(Context);

            var request = new GetTodoItemRequest { Id = requestId };
            var response = await handler.HandleAsync(request);

            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Notifications.Count);

            var notification = response.Notifications.Single();
            AssertNotFoundNotification(requestId, notification);

            VerifyTodoItem.AssertTodoItem(expectedTodoItemEntity, response.Item);
        }

        private static void AssertNotFoundNotification(int requestId, Notification notification)
        {
            Assert.IsInstanceOfType(notification, typeof(NotFoundNotification));
            Assert.AreEqual(GetTodoItemRequestHandler.TodoItemNotFoundNotificationId, notification.Id);
            Assert.AreEqual($"TodoItem {requestId} was not found.", notification.Detail);
            Assert.AreEqual(Notification.SeverityType.Error, notification.Severity);
        }
    }
}
