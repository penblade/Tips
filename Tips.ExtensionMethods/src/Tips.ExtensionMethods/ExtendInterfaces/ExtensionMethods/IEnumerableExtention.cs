using System.Linq;

namespace System.Collections.Generic
{
    public static class EnumerableExtension
    {
        // TODO: https://stackoverflow.com/questions/277150/define-an-extension-method-for-ienumerablet-which-returns-ienumerablet

        public static IEnumerable<T> GetEveryXEntries<T>(this IEnumerable<T> items, int x)
        {
            var itemList = items.ToList();
            for (var i = 0; i < itemList.Count(); i++)
            {
                if (i % x == 0)
                {
                    yield return itemList[i];
                }
            }
        }
    }
}
