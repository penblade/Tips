namespace Tips.Pipeline
{
    internal class LogFormatter
    {
        public static string FormatForLogging(string message) => message.Replace("{", "{{").Replace("}", "}}");
    }
}
