using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Tests.Support
{
    internal static class VerifyTodoItem
    {
        public static void AssertTodoItem(TodoItemEntity expected, TodoItem actual)
        {
            if (expected == null && actual == null) return;

            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.IsComplete, actual.IsComplete);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Priority, actual.Priority);
        }

        public static void AssertTodoItem(TodoItemEntity expected, TodoItemEntity actual)
        {
            if (expected == null && actual == null) return;

            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.IsComplete, actual.IsComplete);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Priority, actual.Priority);
        }

        public static bool AreEqualResponse(Response<TodoItemEntity> response1, Response<TodoItemEntity> response2) =>
            response1.Item.Id == response2.Item.Id
            && response1.Item.Name == response2.Item.Name
            && response1.Item.Description == response2.Item.Description
            && response1.Item.Priority == response2.Item.Priority
            && response1.Item.IsComplete == response2.Item.IsComplete
            && response1.Item.Reviewer == response2.Item.Reviewer;

        public static void AssertAreEqualResponse(Response<TodoItemEntity> expectedResponse, Response<TodoItem> actualResponse)
        {
            Assert.AreEqual(expectedResponse.Item.Id, actualResponse.Item.Id);
            Assert.AreEqual(expectedResponse.Item.Name, actualResponse.Item.Name);
            Assert.AreEqual(expectedResponse.Item.Description, actualResponse.Item.Description);
            Assert.AreEqual(expectedResponse.Item.Priority, actualResponse.Item.Priority);
            Assert.AreEqual(expectedResponse.Item.IsComplete, actualResponse.Item.IsComplete);
        }
    }
}
