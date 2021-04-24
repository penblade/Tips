using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace TodoItems.Tests.Support
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
    }
}
