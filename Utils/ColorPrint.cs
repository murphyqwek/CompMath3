namespace CompMath3.Utils
{
    public static class ColorPrint
    {
        public static void Print(string message, ConsoleColor color, bool isNewLine = true)
        {
            Console.ForegroundColor = color;
            if (isNewLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Print(message, ConsoleColor.Red);
        }
    }
}
