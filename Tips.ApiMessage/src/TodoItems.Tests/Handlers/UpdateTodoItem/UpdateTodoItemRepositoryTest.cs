using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Support.Tests;
using Tips.Pipeline;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using TodoItems.Tests.Context;
using TodoItems.Tests.Support;

namespace TodoItems.Tests.Handlers.UpdateTodoItem
{
    [TestClass]
    public class UpdateTodoItemRepositoryTest : WithContext
    {
        [TestMethod]
        public async Task SaveAsyncSuccessTest()
        {
            // Arrange
            var todoItemEntities = await PopulateTodoItems(1);
            var expectedResponse = new Response<TodoItemEntity> { Item = todoItemEntities.Single().Clone() };
            expectedResponse.Item.Description = "Updated";

            // The response is changed in the SaveAsync, so don't use the same expected response as the one used in the method.
            var response = expectedResponse.Clone();

            // Act
            var repository = new UpdateTodoItemRepository(Context);
            await repository.SaveAsync(response);

            // Assert
            var actualTodoItemEntity = await GetTodoItem(1);
            VerifyTodoItem.AssertTodoItem(expectedResponse.Item, actualTodoItemEntity);
            VerifyTodoItem.AssertTodoItem(expectedResponse.Item, response.Item);
        }

        [TestMethod]
        [DynamicData(nameof(SetupSaveAsyncGuardAgainstNullTest), DynamicDataSourceType.Method)]
        public async Task SaveAsyncGuardAgainstNullTest(string scenario, object response, string parameterName)
        {
            var expectedResponse = (Response<TodoItemEntity>) response;

            var repository = new UpdateTodoItemRepository(Context);

            var expectedException = new ArgumentNullException(parameterName);
            await Verify.ThrowsExceptionAsync(() => repository.SaveAsync(expectedResponse), expectedException);
        }

        private static IEnumerable<object[]> SetupSaveAsyncGuardAgainstNullTest()
        {
            yield return new object[] { "Response == null", null, "response" };
            yield return new object[] { "Response.Item == null", new Response<TodoItemEntity> {Item = null}, "Item" };
        }

        [TestMethod]
        public async Task SaveAsyncItemDoesNotExistTest()
        {
            var response = new Response<TodoItemEntity> { Item = TodoItemFactory.CreateTodoItemEntity(1) };

            var repository = new UpdateTodoItemRepository(Context);
            await repository.SaveAsync(response);

            AssertResponse(response, UpdateTodoItemRepository.TodoItemNotFoundNotificationId, "TodoItem 1 was not found.");
        }

        // Can't mock the context or any methods to force the item to not exist
        // after context Find but before the context Save.
        // SaveAsyncItemDoesNotExistWhenSavingTest()

        private static void AssertResponse(Response<TodoItemEntity> response, string notificationId, string notificationDetail)
        {
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Item);
            Assert.IsNotNull(response.Notifications);
            Assert.AreEqual(1, response.Notifications.Count);

            var notification = response.Notifications.Single();
            Assert.IsInstanceOfType(notification, typeof(NotFoundNotification));
            Assert.AreEqual(notificationId, notification.Id);
            Assert.AreEqual(notificationDetail, notification.Detail);
        }
    }
}
