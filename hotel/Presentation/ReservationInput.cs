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

            Console.WriteLine("  Please enter a whole number, or q to cancel.");
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

            Console.WriteLine("  Please enter a date as yyyy-MM-dd, or q to cancel.");
        }
    }

    public static string? ReadText(string prompt)
    {
        Console.Write($"  {prompt}");
        string? input = Console.ReadLine().Trim().ToLower();
        if (input == null || input.Equals("q", StringComparison.OrdinalIgnoreCase))
            return null;

        return input;
    }
}
