using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tips.ExtensionMethods.ExtendInterfaces.Tests
{
    [TestClass]
    public class EnumerableExtensionTests
    {
        [TestMethod]
        [DataRow(1, 100, 1, 100)]
        [DataRow(1, 100, 2, 50)]
        [DataRow(1, 100, 4, 25)]
        [DataRow(1, 100, 5, 20)]
        [DataRow(1, 100, 10, 10)]
        [DataRow(1, 100, 20, 5)]
        [DataRow(1, 100, 25, 4)]
        [DataRow(1, 100, 50, 2)]
        [DataRow(1, 100, 100, 1)]
        public void GetEveryXEntries(int startInteger, int totalEntries, int everyXEntries, int expectedEntries)
        {
            var list = Enumerable.Range(startInteger, totalEntries);
            var filteredList = list.GetEveryXEntries(everyXEntries);
            Assert.AreEqual(expectedEntries, filteredList.Count());
        }

        [TestMethod]
        [DataRow(1, 100, 1, 100)]
        [DataRow(1, 100, 2, 50)]
        [DataRow(1, 100, 4, 25)]
        [DataRow(1, 100, 5, 20)]
        [DataRow(1, 100, 10, 10)]
        [DataRow(1, 100, 20, 5)]
        [DataRow(1, 100, 25, 4)]
        [DataRow(1, 100, 50, 2)]
        [DataRow(1, 100, 100, 1)]
        public void UseWhereClauseOnListOfInt(int startInteger, int totalEntries, int everyXEntries, int expectedEntries)
        {
            var list = Enumerable.Range(startInteger, totalEntries);
            var filteredList = list.Where(x => x % everyXEntries == 0);
            Assert.AreEqual(expectedEntries, filteredList.Count());
        }

        [TestMethod]
        [DataRow(1, 100, 1, 100)]
        [DataRow(1, 100, 2, 50)]
        [DataRow(1, 100, 4, 25)]
        [DataRow(1, 100, 5, 20)]
        [DataRow(1, 100, 10, 10)]
        [DataRow(1, 100, 20, 5)]
        [DataRow(1, 100, 25, 4)]
        [DataRow(1, 100, 50, 2)]
        [DataRow(1, 100, 100, 1)]
        public void UseWhereClauseOnAnonymousType(int startInteger, int totalEntries, int everyXEntries, int expectedEntries)
        {
            var list = Enumerable.Range(startInteger, totalEntries).Select(x => new { entry = x});
            var filteredList = list.Where(anonymous => anonymous.entry % everyXEntries == 0);
            Assert.AreEqual(expectedEntries, filteredList.Count());
        }
    }
}
