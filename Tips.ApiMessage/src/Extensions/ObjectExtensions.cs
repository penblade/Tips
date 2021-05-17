using System.Text.Json;

namespace Extensions
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T item) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(item));
    }
}
