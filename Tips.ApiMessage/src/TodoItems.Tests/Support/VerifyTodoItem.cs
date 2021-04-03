using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace TodoItems.Tests.Support
{
    internal static class VerifyTodoItem
    {
        public static void AssertTodoItem(TodoItemEntity todoItemEntity, TodoItem todoItem)
        {
            if (todoItemEntity == null && todoItem == null) return;

            Assert.AreEqual(todoItemEntity?.Id, todoItem?.Id);
            Assert.AreEqual(todoItemEntity?.Description, todoItem?.Description);
            Assert.AreEqual(todoItemEntity?.IsComplete, todoItem?.IsComplete);
            Assert.AreEqual(todoItemEntity?.Name, todoItem?.Name);
            Assert.AreEqual(todoItemEntity?.Priority, todoItem?.Priority);
        }
    }
}
