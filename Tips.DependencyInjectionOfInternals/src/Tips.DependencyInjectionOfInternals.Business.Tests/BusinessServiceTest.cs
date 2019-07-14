using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.DependencyInjectionOfInternals.Business.Commands;
using Tips.DependencyInjectionOfInternals.Business.Configuration;
using Tips.DependencyInjectionOfInternals.Business.Models;
using CommandType = Tips.DependencyInjectionOfInternals.Business.Models.CommandType;

namespace Tips.DependencyInjectionOfInternals.Business.Tests
{
    [TestClass]
    public class BusinessServiceTest
    {
        private readonly BusinessConfiguration _expectedBusinessConfiguration;
        private readonly List<ICommand> _expectedCommands;

        public BusinessServiceTest()
        {
            _expectedBusinessConfiguration = new BusinessConfiguration
            {
                ConnectionString = "ConnectionStringTest",
                DocumentPath = "DocumentPathTest",
                IocFiles = new List<string>
                {
                    "IocFile1",
                    "IocFile2",
                    "IocFile3"
                }
            };

            _expectedCommands = new List<ICommand>
            {
                new CommandB(),
                new CommandA(_expectedBusinessConfiguration),
                new CommandC(_expectedBusinessConfiguration)
            };
        }

        [TestMethod]
        public void WhenCreateThenMessages()
        {
            var expectedMessage1 = "Mock Command was processed.  1";
            var expectedMessage2 = "Mock Command was processed.  2";
            var expectedMessage3 = "Mock Command was processed.  3";

            var mockCommand1 = new Mock<ICommand>();
            mockCommand1.Setup(x => x.Process(It.IsAny<ProcessRequest>())).Returns(expectedMessage1);

            var mockCommand2 = new Mock<ICommand>();
            mockCommand2.Setup(x => x.Process(It.IsAny<ProcessRequest>())).Returns(expectedMessage2);

            var mockCommand3 = new Mock<ICommand>();
            mockCommand3.Setup(x => x.Process(It.IsAny<ProcessRequest>())).Returns(expectedMessage3);

            var mockCommandFactory = new Mock<ICommandFactory>();
            mockCommandFactory.Setup(x => x.Create(It.IsAny<Models.CommandType>()))
                .Returns(new List<ICommand> { mockCommand1.Object, mockCommand2.Object, mockCommand3.Object });

            var businessService = new BusinessService(mockCommandFactory.Object);
            var response = businessService.Process(new ProcessRequest());

            Assert.AreEqual(expectedMessage1, response.Messages[0]);
            Assert.AreEqual(expectedMessage2, response.Messages[1]);
            Assert.AreEqual(expectedMessage3, response.Messages[2]);
        }
    }
}
