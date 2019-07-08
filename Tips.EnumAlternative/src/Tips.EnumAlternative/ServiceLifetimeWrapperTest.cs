using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.EnumAlternative
{
    [TestClass]
    public class ServiceLifetimeWrapperTest
    {
        [TestMethod]
        [DataRow("notSet")]
        [DataRow("TranSient")]
        [DataRow("SCOPED")]
        [DataRow("S1ngleton")]
        public void FromNameWhenPropertyDoesNotExistThenThrowException(string expectedServiceLifeTimeWrapperName)
        {
            Assert.ThrowsException<ArgumentException>(() =>
                    ServiceLifetimeWrapper.FromName(expectedServiceLifeTimeWrapperName),
                    $"Could not get a ServiceLifetimeWrapper from name: {expectedServiceLifeTimeWrapperName}"
                );
        }

        [TestMethod]
        [DataRow("NotSet")]
        [DataRow("Transient")]
        [DataRow("Scoped")]
        [DataRow("Singleton")]
        public void FromNameWhenPropertyDoesExistThenReturnInstance(string expectedServiceLifeTimeWrapperName)
        {
            var actualServiceLifeTimeWrapper = ServiceLifetimeWrapper.FromName(expectedServiceLifeTimeWrapperName);
            Assert.AreEqual(expectedServiceLifeTimeWrapperName, actualServiceLifeTimeWrapper.Name);
        }

        [TestMethod]
        [DataRow("NotSet")]
        [DataRow("Transient")]
        [DataRow("Scoped")]
        [DataRow("Singleton")]
        public void FromNameWhenPropertyDoesExistThenReturnSingleInstance(string expectedServiceLifeTimeWrapperName)
        {
            var list = new List<ServiceLifetimeWrapper>();
            for (var i = 0; i < 100; i++)
            {
                list.Add(ServiceLifetimeWrapper.FromName(expectedServiceLifeTimeWrapperName));
            }

            var firstItem = list.First();
            Assert.AreEqual(expectedServiceLifeTimeWrapperName, firstItem.Name);

            foreach (var actualServiceLifeTimeWrapper in list)
            {
                Assert.AreEqual(firstItem.Name, actualServiceLifeTimeWrapper.Name);
                Assert.AreSame(firstItem, actualServiceLifeTimeWrapper);
            }
        }

        [TestMethod]
        [DataRow("NotSet")]
        [DataRow("Transient")]
        [DataRow("Scoped")]
        [DataRow("Singleton")]
        public void FromNameWhenPropertyDoesExistAndRunInParallelThenReturnSingleInstance(
            string expectedServiceLifeTimeWrapperName)
        {
            var queue = new ConcurrentQueue<ServiceLifetimeWrapper>();

            Parallel.For(0, 1000000,
                (i) => queue.Enqueue(ServiceLifetimeWrapper.FromName(expectedServiceLifeTimeWrapperName)));

            var firstItem = queue.First();
            foreach (var actualServiceLifeTimeWrapper in queue)
            {
                Assert.AreEqual(expectedServiceLifeTimeWrapperName, actualServiceLifeTimeWrapper.Name);
                Assert.AreSame(firstItem, actualServiceLifeTimeWrapper);
            }
        }

        [TestMethod]
        [DataRow("NotSet")]
        [DataRow("Transient")]
        [DataRow("Scoped")]
        [DataRow("Singleton")]
        public void FromNameThenSwitchOnName(string expectedServiceLifeTimeWrapperName)
        {
            var actualServiceLifeTimeWrapper = ServiceLifetimeWrapper.FromName(expectedServiceLifeTimeWrapperName);
            var actualNameServiceLifeTimeWrapperName = string.Empty;

            switch (actualServiceLifeTimeWrapper.Name)
            {
                case ServiceLifetimeWrapper.NotSet:
                    actualNameServiceLifeTimeWrapperName = expectedServiceLifeTimeWrapperName;
                    break;
                case ServiceLifetimeWrapper.Scoped:
                    actualNameServiceLifeTimeWrapperName = expectedServiceLifeTimeWrapperName;
                    break;
                case ServiceLifetimeWrapper.Transient:
                    actualNameServiceLifeTimeWrapperName = expectedServiceLifeTimeWrapperName;
                    break;
                case ServiceLifetimeWrapper.Singleton:
                    actualNameServiceLifeTimeWrapperName = expectedServiceLifeTimeWrapperName;
                    break;
                default:
                    Assert.Fail("Switch case not found.");
                    break;
            }
            Assert.AreEqual(expectedServiceLifeTimeWrapperName, actualNameServiceLifeTimeWrapperName);
        }

        [TestMethod]
        [DataRow("NotSet")]
        [DataRow("Transient")]
        [DataRow("Scoped")]
        [DataRow("Singleton")]
        public void FromNameThenSwitchOnConstant(string expectedServiceLifeTimeWrapperName)
        {
            var actualNameServiceLifeTimeWrapperName = string.Empty;
            switch (expectedServiceLifeTimeWrapperName)
            {
                case ServiceLifetimeWrapper.NotSet:
                    actualNameServiceLifeTimeWrapperName = expectedServiceLifeTimeWrapperName;
                    break;
                case ServiceLifetimeWrapper.Scoped:
                    actualNameServiceLifeTimeWrapperName = expectedServiceLifeTimeWrapperName;
                    break;
                case ServiceLifetimeWrapper.Transient:
                    actualNameServiceLifeTimeWrapperName = expectedServiceLifeTimeWrapperName;
                    break;
                case ServiceLifetimeWrapper.Singleton:
                    actualNameServiceLifeTimeWrapperName = expectedServiceLifeTimeWrapperName;
                    break;
                default:
                    Assert.Fail("Switch case not found.");
                    break;
            }
            Assert.AreEqual(expectedServiceLifeTimeWrapperName, actualNameServiceLifeTimeWrapperName);
        }
    }
}