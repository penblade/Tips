using System.Text.Json;

namespace Tips.ApiMessage.TodoItems.Mappers
{
    internal class GenericMapper
    {
        public static TTarget Map<TSource, TTarget>(TSource todoItem) => JsonSerializer.Deserialize<TTarget>(JsonSerializer.Serialize(todoItem));
    }
}
