namespace Tips.ApiMessage.Pipeline
{
    public class LogFormatter
    {
        public static string FormatForLogging(string message) => message.Replace("{", "{{").Replace("}", "}}");
    }
}
