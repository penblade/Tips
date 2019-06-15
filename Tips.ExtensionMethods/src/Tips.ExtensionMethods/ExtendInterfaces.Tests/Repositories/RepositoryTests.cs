using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.ExtensionMethods.ExtendInterfaces.ExtensionMethods;
using Tips.ExtensionMethods.ExtendInterfaces.Models;

namespace Tips.ExtensionMethods.ExtendInterfaces.Tests.Repositories
{
    [TestClass]
    public class RepositoryTests
    {
        private readonly List<Item> _allItems = new List<Item>();
        private readonly IEnumerable<Item> _expectedItems;

        public RepositoryTests()
        {
            var testItemFactory = new TestItemFactory();
            _expectedItems = testItemFactory.CreateExpectedUniqueItems();

            var factory = new MockRepositoryFactory(testItemFactory);
            var repository1 = factory.CreateMockDatabaseRepository();
            var repository2 = factory.CreateMockFileRepository();
            var repository3 = factory.CreateMockStreamRepository();

            _allItems.AddRange(repository1.GetItems());
            _allItems.AddRange(repository2.GetItems());
            _allItems.AddRange(repository3.GetItems());
        }

        [TestMethod]
        public void GetItemsDistinctTest()
        {
            var actualItems = _allItems.GroupBy(item => (item.Id, item.Name, item.IsActive)).Select(item => item.First());
            AssertDistinctItems(_expectedItems.ToList(), actualItems.ToList());
        }

        [TestMethod]
        public void GetItemsDistinctExtensionTest()
        {
            var actualItems = _allItems.DistinctItems();
            AssertDistinctItems(_expectedItems.ToList(), actualItems.ToList());
        }

        private static void AssertDistinctItems(IReadOnlyList<Item> expectedItems, IReadOnlyList<Item> actualItems)
        {
            Assert.IsNotNull(expectedItems);
            Assert.IsNotNull(actualItems);

            Assert.AreEqual(expectedItems.Count, actualItems.Count);

            for (var i = 0; i < expectedItems.Count; i++)
            {
                Assert.AreEqual(expectedItems[i].Id, actualItems[i].Id);
                Assert.AreEqual(expectedItems[i].Name, actualItems[i].Name);
                Assert.AreEqual(expectedItems[i].IsActive, actualItems[i].IsActive);
            }
        }
    }
}
