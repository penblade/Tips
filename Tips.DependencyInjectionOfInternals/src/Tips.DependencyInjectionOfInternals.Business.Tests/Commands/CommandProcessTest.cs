using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.DependencyInjectionOfInternals.Business.Commands;
using Tips.DependencyInjectionOfInternals.Business.Configuration;
using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business.Tests.Commands
{
    [TestClass]
    public class CommandProcessTest
    {
        private readonly BusinessConfiguration _expectedBusinessConfiguration;
        private readonly List<ICommand> _expectedCommands;

        public CommandProcessTest()
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
        [DataRow("CommandA", CommandType.CommandA, "CommandA was processed.  ConnectionString: {0}")]
        [DataRow("CommandB", CommandType.CommandB, "CommandB was processed.")]
        [DataRow("CommandC", CommandType.CommandC, "CommandC was processed.  DocumentPath: {1}")]
        public void CommandWhenProcessThenMessage(string commandTypeName, CommandType expectedCommandType, string expectedMessage)
        {
            var processRequest = new ProcessRequest
            {
                CommandType = expectedCommandType
            };

            var command = _expectedCommands.Single(x => x.GetType().Name == commandTypeName);
            var message = command.Process(processRequest);

            var expected = string.Format(expectedMessage, _expectedBusinessConfiguration.ConnectionString, _expectedBusinessConfiguration.DocumentPath);
            Assert.AreEqual(expected, message);
        }

        [TestMethod]
        [DataRow(CommandType.CommandA, 1, new[] { "CommandA" })]

        [DataRow(CommandType.CommandB, 1, new[] { "CommandB" })]
        [DataRow(CommandType.CommandC, 1, new[] { "CommandC" })]
        [DataRow(CommandType.All, 1, new[] { "CommandB", "CommandA", "CommandC" })]
        [DataRow(CommandType.None, 1, new string[] { })]
        public void CommandFactoryWhenCreate(CommandType expectedCommandType, int fixForDataRowArray, string[] expectedCommandNames)
        {
            var services = new ServiceCollection();
            services.AddSingleton(typeof(BusinessConfiguration));
            services.AddScoped(typeof(ICommand), typeof(CommandB));
            services.AddScoped(typeof(ICommand), typeof(CommandA));
            services.AddScoped(typeof(ICommand), typeof(CommandC));
            var serviceProvider = services.BuildServiceProvider();
            
            var factory = new CommandFactory(serviceProvider);

            var actualCommands = factory.Create(expectedCommandType).ToList();

            Assert.AreEqual(expectedCommandNames.Length, actualCommands.Count);
            for (var i = 0; i < actualCommands.Count; i++)
            {
                Assert.AreEqual(expectedCommandNames[i], actualCommands[i].GetType().Name);
            }
        }
    }
}
