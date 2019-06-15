using System.Collections.Generic;
using Tips.ExtensionMethods.ExtendInterfaces.Models;

namespace Tips.ExtensionMethods.ExtendInterfaces.Tests.Repositories
{
    internal interface ITestItemFactory
    {
        IEnumerable<Item> CreateExpectedUniqueItems();
        IEnumerable<Item> CreateDatabaseItems();
        IEnumerable<Item> CreateFileItems();
        IEnumerable<Item> CreateStreamItems();
    }
}