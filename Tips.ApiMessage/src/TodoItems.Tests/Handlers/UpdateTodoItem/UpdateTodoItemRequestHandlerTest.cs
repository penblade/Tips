using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Models;
using TodoItems.Tests.Support;

namespace TodoItems.Tests.Handlers.UpdateTodoItem
{
    [TestClass]
    public class UpdateTodoItemRequestHandlerTest
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task HandleAsyncHasErrorTest(bool updateRulesHasError)
        {
            var request = new UpdateTodoItemRequest();

            var updateRules = RuleFactory.CreateEmptyListOfUpdateRules().ToList();
            updateRules.Add(RuleFactory.CreateMockUpdateRule());

            var mockUpdateRulesFactory = new Mock<IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>>>();
            mockUpdateRulesFactory.Setup(rulesFactory => rulesFactory.Create()).Returns(updateRules);

            var saveRules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            saveRules.Add(RuleFactory.CreateMockSaveRule());

            var mockSaveRulesFactory = new Mock<IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>>();
            mockSaveRulesFactory.Setup(rulesFactory => rulesFactory.Create()).Returns(saveRules);

            var errorNotification = Notification.CreateError("1", "error");

            var mockRulesEngine = new Mock<IRulesEngine>();

            // Setup generic mock setups first and then more specific.
            SetupMockProcessAsyncWithSaveRules(mockRulesEngine, request, saveRules,
                callbackResponse => ProcessCallBackResponseToAddNotification(!updateRulesHasError, callbackResponse, errorNotification));
            SetupMockProcessAsyncWithUpdateRules(mockRulesEngine, request, updateRules,
                callbackResponse => ProcessCallBackResponseToAddNotification(updateRulesHasError, callbackResponse, errorNotification));

            var mockUpdateTodoItemRepository = new Mock<IUpdateTodoItemRepository>();
            var mockLogger = new Mock<ILogger<UpdateTodoItemRequestHandler>>();

            var handler = new UpdateTodoItemRequestHandler(mockRulesEngine.Object, mockUpdateRulesFactory.Object, mockSaveRulesFactory.Object, mockUpdateTodoItemRepository.Object, mockLogger.Object);
            var actualResponse = await handler.HandleAsync(request);

            mockUpdateRulesFactory.Verify(rulesFactory => rulesFactory.Create(), Times.Once);
            mockSaveRulesFactory.Verify(rulesFactory => rulesFactory.Create(),
                updateRulesHasError ? Times.Never : Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), updateRules), Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), saveRules),
                updateRulesHasError ? Times.Never : Times.Once);

            mockUpdateRulesFactory.VerifyNoOtherCalls();
            mockSaveRulesFactory.VerifyNoOtherCalls();
            mockRulesEngine.VerifyNoOtherCalls();
            mockUpdateTodoItemRepository.VerifyNoOtherCalls();
            mockLogger.VerifyNoOtherCalls();

            Assert.IsNotNull(actualResponse);
            Assert.AreEqual(1, actualResponse.Notifications.Count);
            Assert.AreSame(errorNotification, actualResponse.Notifications.Single());
        }

        private static void ProcessCallBackResponseToAddNotification(bool updateRulesHasError, Response callbackResponse, Notification errorNotification)
        {
            if (updateRulesHasError)
            {
                callbackResponse.Notifications.Add(errorNotification);
            }
        }

        private static void SetupMockProcessAsyncWithSaveRules(Mock<IRulesEngine> mockRulesEngine, UpdateTodoItemRequest request,
            IReadOnlyCollection<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>> saveRules, Action<Response<TodoItemEntity>> callBackMethod)
        {
            mockRulesEngine.Setup(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), saveRules))
                .Callback<Request<TodoItem>, Response<TodoItemEntity>, IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>>
                ((callbackRequest, callbackResponse, callbackRules) =>
                {
                    callBackMethod(callbackResponse);
                });
        }

        private static void SetupMockProcessAsyncWithUpdateRules(Mock<IRulesEngine> mockRulesEngine, UpdateTodoItemRequest request,
            IReadOnlyCollection<IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>> updateRules, Action<Response<TodoItemEntity>> callBackMethod)
        {
            mockRulesEngine.Setup(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), updateRules))
                .Callback<UpdateTodoItemRequest, Response<TodoItemEntity>, IEnumerable<IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>>>
                ((callbackRequest, callbackResponse, callbackRules) =>
                {
                    callBackMethod(callbackResponse);
                });
        }

        [TestMethod]
        public async Task HandleAsyncSuccessTest()
        {
            var request = new UpdateTodoItemRequest();

            var expectedResponse = CreateExpectedResponse();

            var updateRules = RuleFactory.CreateEmptyListOfUpdateRules().ToList();
            updateRules.Add(RuleFactory.CreateMockUpdateRule());

            var mockUpdateRulesFactory = new Mock<IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>>>();
            mockUpdateRulesFactory.Setup(rulesFactory => rulesFactory.Create()).Returns(updateRules);

            var saveRules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            saveRules.Add(RuleFactory.CreateMockSaveRule());

            var mockSaveRulesFactory = new Mock<IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>>();
            mockSaveRulesFactory.Setup(rulesFactory => rulesFactory.Create()).Returns(saveRules);

            var mockRulesEngine = new Mock<IRulesEngine>();

            // Setup generic mock setups first and then more specific.
            SetupMockProcessAsyncWithSaveRules(mockRulesEngine, request, saveRules,
                callbackResponse => ProcessCallBackResponseToCloneTodoItem(callbackResponse, expectedResponse));
            SetupMockProcessAsyncWithUpdateRules(mockRulesEngine, request, updateRules,
                callbackResponse => ProcessCallBackResponseToCloneTodoItem(callbackResponse, expectedResponse));

            mockRulesEngine.Setup(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), saveRules))
                .Callback<Request<TodoItem>, Response<TodoItemEntity>, IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>>
                ((callbackRequest, callbackResponse, callbackRules) =>
                {
                    callbackResponse.Item = expectedResponse.Clone().Item;
                });

            var mockUpdateTodoItemRepository = new Mock<IUpdateTodoItemRepository>();
            mockUpdateTodoItemRepository.Setup(repository =>
                repository.SaveAsync(It.Is<Response<TodoItemEntity>>(response => VerifyTodoItem.AreEqualResponse(expectedResponse, response))));

            var mockLogger = new Mock<ILogger<UpdateTodoItemRequestHandler>>();

            var handler = new UpdateTodoItemRequestHandler(mockRulesEngine.Object, mockUpdateRulesFactory.Object, mockSaveRulesFactory.Object, mockUpdateTodoItemRepository.Object, mockLogger.Object);
            var actualResponse = await handler.HandleAsync(request);

            mockUpdateRulesFactory.Verify(rulesFactory => rulesFactory.Create(), Times.Once);
            mockSaveRulesFactory.Verify(rulesFactory => rulesFactory.Create(), Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), updateRules), Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), saveRules), Times.Once);

            mockUpdateTodoItemRepository.Verify(repository =>
                repository.SaveAsync(It.Is<Response<TodoItemEntity>>(response => VerifyTodoItem.AreEqualResponse(expectedResponse, response))), Times.Once);

            mockUpdateRulesFactory.VerifyNoOtherCalls();
            mockSaveRulesFactory.VerifyNoOtherCalls();
            mockRulesEngine.VerifyNoOtherCalls();
            mockUpdateTodoItemRepository.VerifyNoOtherCalls();

            // Test critical behavior.  If the logging here is critical, then verify the calls.
            // See examples of ILogger testing in other parts of this app.
            mockLogger.VerifyAll();

            Assert.IsNotNull(actualResponse);
            Assert.IsFalse(actualResponse.HasErrors());
            Assert.AreEqual(0, actualResponse.Notifications.Count);
        }

        private static void ProcessCallBackResponseToCloneTodoItem(Response<TodoItemEntity> callbackResponse, Response<TodoItemEntity> expectedResponse) =>
            callbackResponse.Item = expectedResponse.Clone().Item;

        private static Response<TodoItemEntity> CreateExpectedResponse() =>
            new()
            {
                Item = new TodoItemEntity
                {
                    Id = 5,
                    Name = "Test Name",
                    Description = "Test Description",
                    Priority = 3,
                    IsComplete = false,
                    Reviewer = "Test Reviewer"
                }
            };
    }
}
