using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Api.Controllers;
using Tips.Pipeline;
using Tips.TodoItems.Handlers.GetTodoItems;
using Tips.TodoItems.Models;

namespace Api.Tests.Controllers
{
    [TestClass]
    public class TodoItemsControllerTest
    {
        [TestMethod]
        public async Task GetTodoItemsTest()
        {
            var mockRequestHandler = new Mock<IRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>>>();
            mockRequestHandler.Setup(handler => handler.HandleAsync(It.IsAny<GetTodoItemsRequest>()));

            var expectedResponse = new Response<List<TodoItem>> {Item = new List<TodoItem> {new()}};

            var mockLoggingBehavior = new Mock<IPipelineBehavior>();

            mockLoggingBehavior.Setup(loggingBehavior =>
                loggingBehavior.HandleAsync(
                    It.IsAny<GetTodoItemsRequest>(),
                    It.Is<RequestHandlerDelegate<Response<List<TodoItem>>>>(
                        requestHandlerDelegate => IsDelegateTargetSameAsMethod(requestHandlerDelegate, mockRequestHandler.Object))))
                .ReturnsAsync(expectedResponse);

            var controller = new TodoItemsController(mockLoggingBehavior.Object);

            var actionResult = await controller.GetTodoItems(mockRequestHandler.Object);

            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));

            var actualActionResult = (OkObjectResult) actionResult;

            Assert.AreSame(expectedResponse.Item, actualActionResult?.Value);

            mockLoggingBehavior.Verify(loggingBehavior =>
                loggingBehavior.HandleAsync(
                    It.IsAny<GetTodoItemsRequest>(),
                    It.Is<RequestHandlerDelegate<Response<List<TodoItem>>>>(
                        requestHandlerDelegate => IsDelegateTargetSameAsMethod(requestHandlerDelegate, mockRequestHandler.Object))), Times.Once);
        }

        private static bool IsDelegateTargetSameAsMethod(
            RequestHandlerDelegate<Response<List<TodoItem>>> requestHandlerDelegate,
            object mockRequestHandler)
        {
            var type = requestHandlerDelegate.Target?.GetType();
            var field = type?.GetField("handler");
            var handler = field?.GetValue(requestHandlerDelegate.Target);
            return Equals(handler, mockRequestHandler);
        }
    }
}
