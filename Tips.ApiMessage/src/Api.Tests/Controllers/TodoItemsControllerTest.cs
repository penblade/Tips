using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //var request = new GetTodoItemsRequest();
            var mockRequestHandler = new Mock<IRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>>>();
            mockRequestHandler.Setup(handler => handler.HandleAsync(It.IsAny<GetTodoItemsRequest>()));

            //var mockRequestHandlerDelegate = new Mock<RequestHandlerDelegate<Response<List<TodoItem>>>>();
            //mockRequestHandlerDelegate.Setup(() => mockRequestHandler.Object);

            var expectedResponse = new Response<List<TodoItem>> {Item = new List<TodoItem> {new()}};

            var mockLoggingBehavior = new Mock<IPipelineBehavior>();

            mockLoggingBehavior.Setup(loggingBehavior =>
                loggingBehavior.HandleAsync(
                    It.IsAny<GetTodoItemsRequest>(),
                    It.IsAny<RequestHandlerDelegate<Response<List<TodoItem>>>>())).ReturnsAsync(expectedResponse);

            var controller = new TodoItemsController(mockLoggingBehavior.Object);

            var actionResult = await controller.GetTodoItems(mockRequestHandler.Object);

            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));

            var actualActionResult = (OkObjectResult) actionResult;

            var expectedActionResult = new OkObjectResult(expectedResponse.Item);
            Assert.AreSame(expectedResponse.Item, actualActionResult?.Value);

            //mockLoggingBehavior.Verify(loggingBehavior =>
            //    loggingBehavior.HandleAsync(
            //        It.IsAny<GetTodoItemsRequest>(),
            //        mockRequestHandlerDelegate.Object), Times.Once());

            //mockLoggingBehavior.Verify(loggingBehavior =>
            //    loggingBehavior.HandleAsync(
            //        It.IsAny<GetTodoItemsRequest>(),
            //        It.Is<RequestHandlerDelegate<Response<List<TodoItem>>>>(() => mockRequestHandler.Object)), Times.Once());

            mockLoggingBehavior.Verify(loggingBehavior =>
                loggingBehavior.HandleAsync(
                    It.IsAny<GetTodoItemsRequest>(),
                    It.IsAny<RequestHandlerDelegate<Response<List<TodoItem>>>>()), Times.Once());
        }
    }
}
