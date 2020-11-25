using System;

namespace Tips.GuardClauses
{
    public static class Guard
    {
        public static void AgainstNull(object input, string parameterName)
        {
            if (input == null) throw new ArgumentNullException(parameterName);
        }
    }
}