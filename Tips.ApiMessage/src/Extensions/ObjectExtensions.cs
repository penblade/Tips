using System.Text.Json;

namespace Tips.Extensions
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T item) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(item));
        public static TTarget Clone<TSource, TTarget>(this TSource item) => JsonSerializer.Deserialize<TTarget>(JsonSerializer.Serialize(item));
    }
}
