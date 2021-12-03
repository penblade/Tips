using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.TodoItems.Handlers.DeleteTodoItem;
using Tips.TodoItems.Tests.Context;

namespace Tips.TodoItems.Tests.Handlers.DeleteTodoItems
{
    [TestClass]
    public class DeleteTodoItemRequestHandlerTest : WithContext
    {
        [TestMethod]
        [DataRow(2, 1)]
        [DataRow(2, 2)]
        public async Task HandleAsyncTest(int totalItems, int requestId)
        {
            await PopulateTodoItems(totalItems);
            var todoItemBeforeDelete = await GetTodoItem(requestId);

            var handler = new DeleteTodoItemRequestHandler(Context);

            var request = new DeleteTodoItemRequest { Id = requestId };
            var response = await handler.HandleAsync(request);

            var todoItemAfterDelete = await GetTodoItem(requestId);

            Assert.IsNotNull(todoItemBeforeDelete);
            Assert.IsNull(todoItemAfterDelete);

            Assert.IsNotNull(response);
            Assert.AreEqual(0, response.Notifications.Count);
        }

        [TestMethod]
        [DataRow(2, 0)]
        [DataRow(2, 3)]
        public async Task HandleAsyncNotFoundTest(int totalItems, int requestId)
        {
            await PopulateTodoItems(totalItems);

            var handler = new DeleteTodoItemRequestHandler(Context);

            var request = new DeleteTodoItemRequest { Id = requestId };
            var response = await handler.HandleAsync(request);

            Assert.IsNotNull(response);
            Assert.AreEqual(1, response.Notifications.Count);

            var notification = response.Notifications.Single();
            AssertNotFoundNotification(requestId, notification);
        }

        private static void AssertNotFoundNotification(int requestId, Notification notification)
        {
            Assert.IsInstanceOfType(notification, typeof(NotFoundNotification));
            Assert.AreEqual(DeleteTodoItemRequestHandler.TodoItemNotFoundNotificationId, notification.Id);
            Assert.AreEqual($"TodoItem {requestId} was not found.", notification.Detail);
            Assert.AreEqual(Notification.SeverityType.Error, notification.Severity);
        }
    }
}
