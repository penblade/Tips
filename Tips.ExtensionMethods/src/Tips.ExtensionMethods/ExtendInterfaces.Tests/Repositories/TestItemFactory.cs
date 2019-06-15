using System.Collections.Generic;
using System.Linq;
using Tips.ExtensionMethods.ExtendInterfaces.Models;

namespace Tips.ExtensionMethods.ExtendInterfaces.Tests.Repositories
{
    internal class TestItemFactory : ITestItemFactory
    {
        public IEnumerable<Item> CreateExpectedUniqueItems() => new List<Item>
        {
            new Item {Id = 1, Name = "A", IsActive = true},
            new Item {Id = 2, Name = "B", IsActive = false},
            new Item {Id = 3, Name = "C", IsActive = true},
            new Item {Id = 4, Name = "D", IsActive = false}, // different
            new Item {Id = 4, Name = "D", IsActive = true},  // different
            new Item {Id = 5, Name = "E", IsActive = true}
        };

        public IEnumerable<Item> CreateDatabaseItems() =>
            CreateExpectedUniqueItems().Where(
                item => new List<int>{ 1, 2, 3 }.Contains(item.Id));

        public IEnumerable<Item> CreateFileItems() =>
            CreateExpectedUniqueItems().Where(
                item => new List<int> { 2, 3, 4 }.Contains(item.Id)
                            && !(item.Id == 4 && item.IsActive));
        public IEnumerable<Item> CreateStreamItems() =>
            CreateExpectedUniqueItems().Where(
                item => new List<int> { 3, 4, 5 }.Contains(item.Id)
                            && !(item.Id == 4 && !item.IsActive));
    }
}
