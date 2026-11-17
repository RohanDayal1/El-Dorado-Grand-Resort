public static class ReservationInput
{
    public static int? ReadInt(string prompt)
    {
        while (true)
        {
            string? input = ReadText(prompt);
            if (input == null)
                return null;

            if (int.TryParse(input, out int number))
                return number;

            Console.WriteLine("  Please enter a whole number, or press Esc to cancel.");
        }
    }

    public static DateTime? ReadDate(string prompt)
    {
        while (true)
        {
            string? input = ReadText(prompt);
            if (input == null)
                return null;

            if (DateTime.TryParse(input, out DateTime date))
                return date.Date;

            Console.WriteLine("  Please enter a date as yyyy-MM-dd, or press Esc to cancel.");
        }
    }

    public static string? ReadText(string prompt)
    {
        Console.Write($"  {prompt}");
        string input = "";

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return null;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input.Trim().ToLower();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }
    }
}
