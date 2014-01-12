using System;

namespace TeamMashup.Tools.Helpers
{
    public static class ConsoleHelper
    {
        public static void WriteLine(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = originalColor;
        }
    }
}
