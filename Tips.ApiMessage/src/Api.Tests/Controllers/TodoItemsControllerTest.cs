using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Api.Controllers;
using Tips.Middleware.ErrorHandling;
using Tips.Pipeline;
using Tips.TodoItems.Handlers.CreateTodoItem;
using Tips.TodoItems.Handlers.DeleteTodoItem;
using Tips.TodoItems.Handlers.GetTodoItem;
using Tips.TodoItems.Handlers.GetTodoItems;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Models;

namespace Tips.Api.Tests.Controllers
{
    [TestClass]
    public class TodoItemsControllerTest
    {
        private const int ExpectedId = 1;

        [TestMethod]
        public async Task GetTodoItemsTest()
        {
            var mockRequestHandler = SetupMockRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>>();

            var expectedResponse = new Response<List<TodoItem>> { Item = new List<TodoItem> { new() { Id = ExpectedId } } };

            var mockLoggingBehavior = SetupMockLoggingBehavior(mockRequestHandler, expectedResponse);

            var controller = new TodoItemsController(mockLoggingBehavior.Object);
            var actionResult = await controller.GetTodoItems(mockRequestHandler.Object);

            VerifyMockLoggingBehavior(mockLoggingBehavior, mockRequestHandler);

            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            var actualActionResult = (OkObjectResult)actionResult;
            Assert.AreSame(expectedResponse.Item, actualActionResult?.Value);
        }

        [TestMethod]
        [DynamicData(nameof(SetupGetTodoItemTest), DynamicDataSourceType.Method)]
        public async Task GetTodoItemTest(string scenario, Response<TodoItem> expectedResponse, Type expectedActionResultType)
        {
            var mockRequestHandler = SetupMockRequestHandler<GetTodoItemRequest, Response<TodoItem>>();

            var mockLoggingBehavior = SetupMockLoggingBehavior(mockRequestHandler, expectedResponse);

            var controller = new TodoItemsController(mockLoggingBehavior.Object);
            var actionResult = await controller.GetTodoItem(mockRequestHandler.Object, ExpectedId);

            VerifyMockLoggingBehavior(mockLoggingBehavior, mockRequestHandler);

            Assert.IsInstanceOfType(actionResult, expectedActionResultType);

            if (expectedActionResultType == typeof(OkObjectResult))
            {
                var actualActionResult = (OkObjectResult)actionResult;
                Assert.AreSame(expectedResponse.Item, actualActionResult?.Value);
            }
        }

        private static IEnumerable<object[]> SetupGetTodoItemTest()
        {
            yield return new object[] { "NotFoundResult Scenario", new Response<TodoItem> { Item = null, Notifications = { new NotFoundNotification() } }, typeof(NotFoundResult) };
            yield return new object[] { "Ok Scenario", new Response<TodoItem> { Item = new TodoItem { Id = ExpectedId } }, typeof(OkObjectResult) };
        }

        [TestMethod]
        [DynamicData(nameof(SetupUpdateTodoItemTest), DynamicDataSourceType.Method)]
        public async Task UpdateTodoItemTest(string scenario, Response expectedResponse, Type expectedActionResultType)
        {
            var mockRequestHandler = SetupMockRequestHandler<UpdateTodoItemRequest, Response>();

            var mockProblemDetailsFactory = new Mock<IProblemDetailsFactory>();

            var mockLoggingBehavior = SetupMockLoggingBehavior(mockRequestHandler, expectedResponse);

            var todoItemRequest = new TodoItem();

            var controller = new TodoItemsController(mockLoggingBehavior.Object);
            var actionResult = await controller.UpdateTodoItem(mockRequestHandler.Object, mockProblemDetailsFactory.Object, ExpectedId, todoItemRequest);

            VerifyMockLoggingBehavior(mockLoggingBehavior, mockRequestHandler);

            if (expectedActionResultType == typeof(BadRequestObjectResult))
            {
                mockProblemDetailsFactory.Verify(factory => factory.BadRequest(expectedResponse.Notifications), Times.Once);
            }

            Assert.IsInstanceOfType(actionResult, expectedActionResultType);
        }

        private static IEnumerable<object[]> SetupUpdateTodoItemTest()
        {
            yield return new object[] { "NotFoundResult Scenario", new Response { Notifications = { new NotFoundNotification() } }, typeof(NotFoundResult) };
            yield return new object[] { "BadRequest Scenario", new Response { Notifications = { Notification.CreateError("1", "error") } }, typeof(BadRequestObjectResult) };
            yield return new object[] { "NoContentResult Scenario", new Response(), typeof(NoContentResult) };
        }

        [TestMethod]
        [DynamicData(nameof(SetupCreateTodoItemTest), DynamicDataSourceType.Method)]
        public async Task CreateTodoItemTest(string scenario, Response<TodoItem> expectedResponse, Type expectedActionResultType)
        {
            var mockRequestHandler = SetupMockRequestHandler<CreateTodoItemRequest, Response<TodoItem>>();

            var mockProblemDetailsFactory = new Mock<IProblemDetailsFactory>();

            var mockLoggingBehavior = SetupMockLoggingBehavior(mockRequestHandler, expectedResponse);

            var todoItemRequest = new TodoItem();

            var controller = new TodoItemsController(mockLoggingBehavior.Object);
            var actionResult = await controller.CreateTodoItem(mockRequestHandler.Object, mockProblemDetailsFactory.Object, todoItemRequest);

            VerifyMockLoggingBehavior(mockLoggingBehavior, mockRequestHandler);

            if (expectedActionResultType == typeof(BadRequestObjectResult))
            {
                mockProblemDetailsFactory.Verify(factory => factory.BadRequest(expectedResponse.Notifications), Times.Once);
            }

            Assert.IsInstanceOfType(actionResult, expectedActionResultType);
            if (expectedActionResultType == typeof(CreatedAtActionResult))
            {
                var actualActionResult = (CreatedAtActionResult)actionResult;
                Assert.AreSame(expectedResponse.Item, actualActionResult?.Value);
                Assert.AreEqual(nameof(TodoItemsController.GetTodoItem), actualActionResult.ActionName);
                Assert.AreEqual((long)ExpectedId, actualActionResult.RouteValues["id"]);
            }
        }

        private static IEnumerable<object[]> SetupCreateTodoItemTest()
        {
            yield return new object[] { "BadRequest Scenario", new Response<TodoItem> { Item = new TodoItem { Id = ExpectedId }, Notifications = { Notification.CreateError("1", "error") } }, typeof(BadRequestObjectResult) };
            yield return new object[] { "CreatedAtAction Scenario", new Response<TodoItem> { Item = new TodoItem { Id = ExpectedId } }, typeof(CreatedAtActionResult) };
        }


        [TestMethod]
        [DynamicData(nameof(SetupDeleteTodoItemTest), DynamicDataSourceType.Method)]
        public async Task DeleteTodoItemTest(string scenario, Response expectedResponse, Type expectedActionResultType)
        {
            var mockRequestHandler = SetupMockRequestHandler<DeleteTodoItemRequest, Response>();

            var mockLoggingBehavior = SetupMockLoggingBehavior(mockRequestHandler, expectedResponse);

            var controller = new TodoItemsController(mockLoggingBehavior.Object);
            var actionResult = await controller.DeleteTodoItem(mockRequestHandler.Object, ExpectedId);

            VerifyMockLoggingBehavior(mockLoggingBehavior, mockRequestHandler);

            Assert.IsInstanceOfType(actionResult, expectedActionResultType);
        }

        private static IEnumerable<object[]> SetupDeleteTodoItemTest()
        {
            yield return new object[] { "NotFoundResult Scenario", new Response { Notifications = { new NotFoundNotification() } }, typeof(NotFoundResult) };
            yield return new object[] { "NoContentResult Scenario", new Response(), typeof(NoContentResult) };
        }

        private static Mock<IRequestHandler<TRequest, TResponse>> SetupMockRequestHandler<TRequest, TResponse>()
        {
            var mockRequestHandler = new Mock<IRequestHandler<TRequest, TResponse>>();
            mockRequestHandler.Setup(handler => handler.HandleAsync(It.IsAny<TRequest>()));
            return mockRequestHandler;
        }

        private Mock<IPipelineBehavior> SetupMockLoggingBehavior<TRequest, TResponse>(
            Mock<IRequestHandler<TRequest, TResponse>> mockRequestHandler,
            TResponse expectedResponse)
        {
            var mockLoggingBehavior = new Mock<IPipelineBehavior>();
            mockLoggingBehavior.Setup(loggingBehavior =>
                loggingBehavior.HandleAsync(
                    It.IsAny<TRequest>(),
                    It.Is<RequestHandlerDelegate<TResponse>>(
                        requestHandlerDelegate =>
                            IsDelegateTargetSameAsMethod(requestHandlerDelegate, mockRequestHandler.Object)))).ReturnsAsync(expectedResponse);

            return mockLoggingBehavior;
        }

        private void VerifyMockLoggingBehavior<TRequest, TResponse>(
            Mock<IPipelineBehavior> mockLoggingBehavior,
            Mock<IRequestHandler<TRequest, TResponse>> mockRequestHandler) =>
            mockLoggingBehavior.Verify(loggingBehavior => loggingBehavior.HandleAsync(
                It.IsAny<TRequest>(),
                It.Is<RequestHandlerDelegate<TResponse>>(
                    requestHandlerDelegate =>
                        IsDelegateTargetSameAsMethod(requestHandlerDelegate, mockRequestHandler.Object))), Times.Once);

        private bool IsDelegateTargetSameAsMethod<TResponse>(
            RequestHandlerDelegate<TResponse> requestHandlerDelegate,
            object mockRequestHandler)
        {
            var type = requestHandlerDelegate.Target?.GetType();
            var field = type?.GetField("handler");
            var handler = field?.GetValue(requestHandlerDelegate.Target);
            return Equals(handler, mockRequestHandler);
        }
    }
}
