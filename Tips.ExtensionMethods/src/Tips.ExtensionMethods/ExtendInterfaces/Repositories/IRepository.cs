using System.Collections.Generic;
using Tips.ExtensionMethods.ExtendInterfaces.Models;

namespace Tips.ExtensionMethods.ExtendInterfaces.Repositories
{
    internal interface IRepository
    {
        IEnumerable<Item> GetItems();
    }
}
