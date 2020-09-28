using System;

namespace Tips.TodoItems.Infrastructure
{
    internal static class Guard
    {
        public static void AgainstNull(object input, string parameterName)
        {
            if (input == null) throw new ArgumentNullException(parameterName);
        }
    }
}