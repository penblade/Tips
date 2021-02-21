using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Support.Tests;
using Tips.Api.Configuration;
using Tips.Pipeline;
using Tips.Rules;
using Tips.Security;
using Tips.TodoItems.Handlers.CreateTodoItems;
using Tips.TodoItems.Handlers.DeleteTodoItems;
using Tips.TodoItems.Handlers.GetTodoItem;
using Tips.TodoItems.Handlers.GetTodoItems;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Models;

namespace Api.Tests.Configuration
{
    [TestClass]
    public class ServiceCollectionExtensionsTest
    {
        [TestMethod]
        public void RegisterDependencies()
        {
            var configurationRootFromJson = new ConfigurationBuilder().AddJsonFile(@"Configuration\appSettings.test.json").Build();

            var serviceCollection = new ServiceCollection();
            DependencyRegistrarSupport.AddScopedLogger(serviceCollection);
            serviceCollection.RegisterDependencies(configurationRootFromJson);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // No clean way of getting the count of ILogger<out TCategoryName>
            // without changing the class implemented for the logger
            // from internal to public, which defeats the purpose.
            // Long term solution is still to create a Facade for the ILogger.

            // In order to verify the internals were actually registered properly
            // we could make them public, but that would defeat the purpose.

            // We could also create verify methods in the dependent Test projects
            // and the references here to call them, but that would mean adding
            // dependencies on each test project.  Again, that would defeat the
            // purpose of doing the tests.

            // Best solution to honor the dependencies while still testing,
            // would be to change ServiceCollectionExtensions to a concrete
            // class that is registered in the Program so it could be
            // injected in the Startup.

            // As a rule of thumb, unit test the critical logic, anything
            // discovered as a bug to verify the issue is fixed.
            // If the infrastructure is critical then, unit test it.
            // If the rules are critical, then unit test the rules.

            // Remember to perform integration tests, manual and automated
            // to ensure your app works as expected and is hooked up properly.

            AssertDependenciesForPipeline(serviceProvider);
            AssertDependenciesForMiddleware(serviceProvider);
            AssertDependenciesForSecurity(serviceProvider);
            AssertDependenciesForRules(serviceProvider);
            AssertDependenciesForTodoItems(serviceProvider);
        }

        private static void AssertDependenciesForPipeline(IServiceProvider serviceProvider) => 
            DependencyRegistrarSupport.AssertServiceIsNotNull<IPipelineBehavior>(serviceProvider);

        private static void AssertDependenciesForMiddleware(IServiceProvider serviceProvider)
        {
            // Everything is internal, so nothing can be tested.
        }

        private static void AssertDependenciesForSecurity(IServiceProvider serviceProvider)
        {
            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<ApiKeyConfiguration, ApiKeyConfiguration>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsNotNull<IApiKeyRepository>(serviceProvider);

            AssertApiKeyConfiguration(serviceProvider);
        }

        private static void AssertDependenciesForRules(IServiceProvider serviceProvider)
        {
            DependencyRegistrarSupport.AssertServiceIsNotNull<IRulesEngine>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsNotNull<IRulesFactory<string, int>>(serviceProvider);
        }

        private static void AssertDependenciesForTodoItems(IServiceProvider serviceProvider)
        {
            // Cannot verify the TodoContext because it is internal only
            
            // We can test the handlers and contracts were setup.
            DependencyRegistrarSupport.AssertServiceIsNotNull<IRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>>>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsNotNull<IRequestHandler<GetTodoItemRequest, Response<TodoItem>>>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsNotNull<IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsNotNull<IRequestHandler<DeleteTodoItemRequest, Response>>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsNotNull<IRequestHandler<UpdateTodoItemRequest, Response>>(serviceProvider);
        }

        private static void AssertApiKeyConfiguration(IServiceProvider serviceProvider)
        {
            var expectedConfiguration = CreateExpectedApiKeyConfiguration();

            var actualConfiguration = serviceProvider.GetService<ApiKeyConfiguration>();
            Assert.AreEqual(expectedConfiguration.ApiHeader, actualConfiguration?.ApiHeader);

            var expectedApiKeys = expectedConfiguration.ApiKeys.ToList();
            var actualApiKeys = actualConfiguration?.ApiKeys.ToList();
            for (var i = 0; i < expectedApiKeys.Count; i++)
            {
                AssertApiKey(expectedApiKeys[i], actualApiKeys?[i]);
            }
        }

        private static void AssertApiKey(ApiKey expectedApiKey, ApiKey actualApiKey)
        {
            Assert.AreEqual(expectedApiKey.Id, actualApiKey.Id);
            Assert.AreEqual(expectedApiKey.Owner, actualApiKey.Owner);
            Assert.AreEqual(expectedApiKey.Key, actualApiKey.Key);
            Assert.AreEqual(expectedApiKey.Created, actualApiKey.Created);
        }

        private static ApiKeyConfiguration CreateExpectedApiKeyConfiguration() =>
            new()
            {
                ApiHeader = "x-api-key",
                ApiKeys = new[]
                {
                    new ApiKey
                    {
                        Id = 1,
                        Owner = "TodoWebsite",
                        Key = "77D4AA72-16F8-47D4-B237-DE285A271BF8",
                        Created = new DateTime(2020, 06, 01)
                    },
                    new ApiKey
                    {
                        Id = 2,
                        Owner = "PostmanTester",
                        Key = "682D073E-3590-4051-A4A7-83CEEBCC5E30",
                        Created = new DateTime(2020, 07, 01)
                    },
                    new ApiKey
                    {
                        Id = 3,
                        Owner = "Client1",
                        Key = "185758D5-136B-45B0-B51B-4D8CEB250FBB",
                        Created = new DateTime(2020, 08, 01)
                    },
                    new ApiKey
                    {
                        Id = 4,
                        Owner = "Client2",
                        Key = "EAEC8089-B6BE-4BA3-B1C1-1A98D4116894",
                        Created = new DateTime(2020, 09, 01)
                    },
                    new ApiKey
                    {
                        Id = 5,
                        Owner = "Client3-DuplicateKey",
                        Key = "EAEC8089-B6BE-4BA3-B1C1-1A98D4116894",
                        Created = new DateTime(2020, 10, 01)
                    }
                }
            };
    }
}
