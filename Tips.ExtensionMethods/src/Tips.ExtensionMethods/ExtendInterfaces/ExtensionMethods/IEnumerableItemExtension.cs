using System.Collections.Generic;
using System.Linq;
using Tips.ExtensionMethods.ExtendInterfaces.Models;

namespace Tips.ExtensionMethods.ExtendInterfaces.ExtensionMethods
{
    internal static class IEnumerableItemExtension
    {
        // Inspiration
        // https://stackoverflow.com/questions/489258/linqs-distinct-on-a-particular-property

        public static IEnumerable<Item> DistinctItems(this IEnumerable<Item> items)
        {
            return items.GroupBy(item => (item.Id, item.Name, item.IsActive)).Select(item => item.First());
        }
    }
}