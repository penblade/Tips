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
using Tips.TodoItems.Handlers.CreateTodoItem;
using Tips.TodoItems.Models;
using TodoItems.Tests.Support;

namespace TodoItems.Tests.Handlers.CreateTodoItem
{
    [TestClass]
    public class CreateTodoItemRequestHandlerTest
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task HandleAsyncHasErrorTest(bool createRulesHasError)
        {
            var request = new CreateTodoItemRequest();

            var createRules = RuleFactory.CreateEmptyListOfCreateRules().ToList();
            createRules.Add(RuleFactory.CreateMockCreateRule());

            var mockCreateRulesFactory = new Mock<IRulesFactory<CreateTodoItemRequest, Response<TodoItemEntity>>>();
            mockCreateRulesFactory.Setup(rulesFactory => rulesFactory.Create()).Returns(createRules);

            var saveRules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            saveRules.Add(RuleFactory.CreateMockSaveRule());

            var mockSaveRulesFactory = new Mock<IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>>();
            mockSaveRulesFactory.Setup(rulesFactory => rulesFactory.Create()).Returns(saveRules);

            var errorNotification = Notification.CreateError("1", "error");

            var mockRulesEngine = new Mock<IRulesEngine>();

            // Setup generic mock setups first and then more specific.
            SetupMockProcessAsyncWithSaveRules(mockRulesEngine, request, saveRules,
                callbackResponse => ProcessCallBackResponseToAddNotification(!createRulesHasError, callbackResponse, errorNotification));
            SetupMockProcessAsyncWithCreateRules(mockRulesEngine, request, createRules, 
                callbackResponse => ProcessCallBackResponseToAddNotification(createRulesHasError, callbackResponse, errorNotification));

            var mockCreateTodoItemRepository = new Mock<ICreateTodoItemRepository>();
            var mockLogger = new Mock<ILogger<CreateTodoItemRequestHandler>>();

            var handler = new CreateTodoItemRequestHandler(mockRulesEngine.Object, mockCreateRulesFactory.Object, mockSaveRulesFactory.Object, mockCreateTodoItemRepository.Object, mockLogger.Object);
            var actualResponse = await handler.HandleAsync(request);

            mockCreateRulesFactory.Verify(rulesFactory => rulesFactory.Create(), Times.Once);
            mockSaveRulesFactory.Verify(rulesFactory => rulesFactory.Create(),
                createRulesHasError ? Times.Never : Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), createRules), Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), saveRules),
                createRulesHasError ? Times.Never : Times.Once);

            mockCreateRulesFactory.VerifyNoOtherCalls();
            mockSaveRulesFactory.VerifyNoOtherCalls();
            mockRulesEngine.VerifyNoOtherCalls();
            mockCreateTodoItemRepository.VerifyNoOtherCalls();
            mockLogger.VerifyNoOtherCalls();

            Assert.IsNotNull(actualResponse);
            Assert.IsNull(actualResponse.Item);
            Assert.AreEqual(1, actualResponse.Notifications.Count);
            Assert.AreSame(errorNotification, actualResponse.Notifications.Single());
        }

        private static void ProcessCallBackResponseToAddNotification(bool createRulesHasError, Response callbackResponse, Notification errorNotification)
        {
            if (createRulesHasError)
            {
                callbackResponse.Notifications.Add(errorNotification);
            }
        }

        private static void SetupMockProcessAsyncWithSaveRules(Mock<IRulesEngine> mockRulesEngine, CreateTodoItemRequest request,
            IReadOnlyCollection<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>> saveRules, Action<Response<TodoItemEntity>> callBackMethod)
        {
            mockRulesEngine.Setup(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), saveRules))
                .Callback<Request<TodoItem>, Response<TodoItemEntity>, IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>>
                ((callbackRequest, callbackResponse, callbackRules) =>
                {
                    callBackMethod(callbackResponse);
                });
        }

        private static void SetupMockProcessAsyncWithCreateRules(Mock<IRulesEngine> mockRulesEngine, CreateTodoItemRequest request,
            IReadOnlyCollection<IBaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>> createRules, Action<Response<TodoItemEntity>> callBackMethod)
        {
            mockRulesEngine.Setup(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), createRules))
                .Callback<CreateTodoItemRequest, Response<TodoItemEntity>, IEnumerable<IBaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>>>
                ((callbackRequest, callbackResponse, callbackRules) =>
                {
                    callBackMethod(callbackResponse);
                });
        }

        [TestMethod]
        public async Task HandleAsyncSuccessTest()
        {
            var request = new CreateTodoItemRequest();

            var expectedResponse = CreateExpectedResponse();

            var createRules = RuleFactory.CreateEmptyListOfCreateRules().ToList();
            createRules.Add(RuleFactory.CreateMockCreateRule());

            var mockCreateRulesFactory = new Mock<IRulesFactory<CreateTodoItemRequest, Response<TodoItemEntity>>>();
            mockCreateRulesFactory.Setup(rulesFactory => rulesFactory.Create()).Returns(createRules);

            var saveRules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            saveRules.Add(RuleFactory.CreateMockSaveRule());

            var mockSaveRulesFactory = new Mock<IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>>();
            mockSaveRulesFactory.Setup(rulesFactory => rulesFactory.Create()).Returns(saveRules);

            var mockRulesEngine = new Mock<IRulesEngine>();

            // Setup generic mock setups first and then more specific.
            SetupMockProcessAsyncWithSaveRules(mockRulesEngine, request, saveRules,
                callbackResponse => ProcessCallBackResponseToCloneTodoItem(callbackResponse, expectedResponse));
            SetupMockProcessAsyncWithCreateRules(mockRulesEngine, request, createRules,
                callbackResponse => ProcessCallBackResponseToCloneTodoItem(callbackResponse, expectedResponse));

            mockRulesEngine.Setup(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), saveRules))
                .Callback<Request<TodoItem>, Response<TodoItemEntity>, IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>>
                ((callbackRequest, callbackResponse, callbackRules) =>
                {
                    callbackResponse.Item = expectedResponse.Clone().Item;
                });

            var mockCreateTodoItemRepository = new Mock<ICreateTodoItemRepository>();
            mockCreateTodoItemRepository.Setup(repository =>
                repository.SaveAsync(It.Is<Response<TodoItemEntity>>(response => VerifyTodoItem.AreEqualResponse(expectedResponse, response))));

            var mockLogger = new Mock<ILogger<CreateTodoItemRequestHandler>>();

            var handler = new CreateTodoItemRequestHandler(mockRulesEngine.Object, mockCreateRulesFactory.Object, mockSaveRulesFactory.Object, mockCreateTodoItemRepository.Object, mockLogger.Object);
            var actualResponse = await handler.HandleAsync(request);

            mockCreateRulesFactory.Verify(rulesFactory => rulesFactory.Create(), Times.Once);
            mockSaveRulesFactory.Verify(rulesFactory => rulesFactory.Create(), Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), createRules), Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), saveRules), Times.Once);

            mockCreateTodoItemRepository.Verify(repository =>
                repository.SaveAsync(It.Is<Response<TodoItemEntity>>(response => VerifyTodoItem.AreEqualResponse(expectedResponse, response))), Times.Once);

            mockCreateRulesFactory.VerifyNoOtherCalls();
            mockSaveRulesFactory.VerifyNoOtherCalls();
            mockRulesEngine.VerifyNoOtherCalls();
            mockCreateTodoItemRepository.VerifyNoOtherCalls();
            
            // Test critical behavior.  If the logging here is critical, then verify the calls.
            // See examples of ILogger testing in other parts of this app.
            mockLogger.VerifyAll();

            Assert.IsNotNull(actualResponse);
            Assert.IsNotNull(actualResponse.Item);
            Assert.IsFalse(actualResponse.HasErrors());
            Assert.AreEqual(0, actualResponse.Notifications.Count);
            VerifyTodoItem.AssertAreEqualResponse(expectedResponse, actualResponse);
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
 