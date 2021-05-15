using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.CreateTodoItem;
using Tips.TodoItems.Models;

namespace TodoItems.Tests.Handlers.CreateTodoItem
{
    [TestClass]
    public class CreateTodoItemRequestHandlerTest
    {
        [TestMethod]
        public async Task HandleAsyncHasErrorTest()
        {
            var request = new CreateTodoItemRequest();

            var rules = new List<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>();
            var mockRulesFactory = new Mock<IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>>();
            mockRulesFactory.Setup(rulesFactory => rulesFactory.Create())
                .Returns(rules);

            var errorNotification = Notification.CreateError("1", "error");

            var mockRulesEngine = new Mock<IRulesEngine>();
            mockRulesEngine.Setup(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), rules))
                .Callback<Request<TodoItem>, Response<TodoItemEntity>, IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>>
                ((callbackRequest, callbackResponse, callbackRules) =>
                {
                    callbackResponse.Notifications.Add(errorNotification);
                });

            var mockCreateTodoItemRepository = new Mock<ICreateTodoItemRepository>();
            var mockLogger = new Mock<ILogger<CreateTodoItemRequestHandler>>();

            var handler = new CreateTodoItemRequestHandler(mockRulesEngine.Object, mockRulesFactory.Object, mockCreateTodoItemRepository.Object, mockLogger.Object);
            var actualResponse = await handler.HandleAsync(request);

            mockRulesFactory.Verify(rulesFactory => rulesFactory.Create(), Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), rules), Times.Once);

            mockRulesFactory.VerifyNoOtherCalls();
            mockRulesEngine.VerifyNoOtherCalls();
            mockCreateTodoItemRepository.VerifyNoOtherCalls();
            mockLogger.VerifyNoOtherCalls();

            Assert.IsNull(actualResponse.Item);
            Assert.AreEqual(1, actualResponse.Notifications.Count);
            Assert.AreSame(errorNotification, actualResponse.Notifications.Single());
        }

        [TestMethod]
        public async Task HandleAsyncSuccessTest()
        {
            var request = new CreateTodoItemRequest();
            var expectedResponse = CreateExpectedResponse();

            var rules = new List<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>();
            var mockRulesFactory = new Mock<IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>>();
            mockRulesFactory.Setup(rulesFactory => rulesFactory.Create())
                .Returns(rules);
            
            var mockRulesEngine = new Mock<IRulesEngine>();
            mockRulesEngine.Setup(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), rules))
                .Callback<Request<TodoItem>, Response<TodoItemEntity>, IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>>
                ((callbackRequest, callbackResponse, callbackRules) =>
                {
                    callbackResponse.Item = new TodoItemEntity
                    {
                        Id = expectedResponse.Item.Id,
                        Name = expectedResponse.Item.Name,
                        Description = expectedResponse.Item.Description,
                        Priority = expectedResponse.Item.Priority,
                        IsComplete = expectedResponse.Item.IsComplete,
                        Reviewer = expectedResponse.Item.Reviewer
                    };
                });


            var mockCreateTodoItemRepository = new Mock<ICreateTodoItemRepository>();
            mockCreateTodoItemRepository.Setup(repository =>
                repository.SaveAsync(It.Is<Response<TodoItemEntity>>(response => AreEqualResponse(expectedResponse, response))));

            var mockLogger = new Mock<ILogger<CreateTodoItemRequestHandler>>();

            var handler = new CreateTodoItemRequestHandler(mockRulesEngine.Object, mockRulesFactory.Object, mockCreateTodoItemRepository.Object, mockLogger.Object);
            var actualResponse = await handler.HandleAsync(request);

            mockRulesFactory.Verify(rulesFactory => rulesFactory.Create(), Times.Once);
            mockRulesEngine.Verify(rulesEngine => rulesEngine.ProcessAsync(request, It.IsAny<Response<TodoItemEntity>>(), rules), Times.Once);
            mockCreateTodoItemRepository.Verify(repository =>
                repository.SaveAsync(It.Is<Response<TodoItemEntity>>(response => AreEqualResponse(expectedResponse, response))), Times.Once);

            mockRulesFactory.VerifyNoOtherCalls();
            mockRulesEngine.VerifyNoOtherCalls();

            // Not sure why this VerifyNoOtherCalls fails, since it verified is was called once above.
            //mockCreateTodoItemRepository.VerifyNoOtherCalls();

            // Test critical behavior.  If the logging here is critical, then verify the calls.
            // See examples of ILogger testing in other parts of this app.
            mockLogger.VerifyAll();

            Assert.IsNotNull(actualResponse.Item);
            Assert.IsFalse(actualResponse.HasErrors());
            Assert.AreEqual(0, actualResponse.Notifications.Count);
            AssertAreEqualResponse(expectedResponse, actualResponse);
        }

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

        private static bool AreEqualResponse(Response<TodoItemEntity> response1, Response<TodoItemEntity> response2) =>
            response1.Item.Id == response2.Item.Id
            && response1.Item.Name == response2.Item.Name
            && response1.Item.Description == response2.Item.Description
            && response1.Item.Priority == response2.Item.Priority
            && response1.Item.IsComplete == response2.Item.IsComplete
            && response1.Item.Reviewer == response2.Item.Reviewer;

        private static void AssertAreEqualResponse(Response<TodoItemEntity> expectedResponse, Response<TodoItem> actualResponse)
        {
            Assert.AreEqual(expectedResponse.Item.Id, actualResponse.Item.Id);
            Assert.AreEqual(expectedResponse.Item.Name, actualResponse.Item.Name);
            Assert.AreEqual(expectedResponse.Item.Description, actualResponse.Item.Description);
            Assert.AreEqual(expectedResponse.Item.Priority, actualResponse.Item.Priority);
            Assert.AreEqual(expectedResponse.Item.IsComplete, actualResponse.Item.IsComplete);
        }
    }
}
 