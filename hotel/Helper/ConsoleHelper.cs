static class ConsoleHelper
{
    public static void ClearScreen()
    {
        if (!Console.IsOutputRedirected)
            Console.Clear();
    }

    public static void ShowHeader(string title)
    {
        const string border = "============================================================";
        ClearScreen();
        Console.WriteLine(border);
        Console.WriteLine($"  EL DORADO GRAND RESORT  |  {title}");
        Console.WriteLine(border);
        Console.WriteLine("  Enter q at any input to cancel.");
        Console.WriteLine();
    }

    public static void WaitForEnter()
    {
        Console.Write("\n  Press Enter to return to the home menu...");
        Console.ReadLine();
    }

    public static void Pause(string message = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.ReadKey(true);
    }
}
