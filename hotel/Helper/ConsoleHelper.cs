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

    public static int SelectionMenu(List<string> options, string title = "", string description = null, List<string> details = null, string information = null, bool allowC = false)
    {
        return ShowSelectionMenu(options, title, description, false, information, details, allowC);
    }

    public static int SelectionMenu(List<string> options, string title, string description, bool horizontal, string information = null, List<string> details = null, bool allowC = false)
    {
        return ShowSelectionMenu(options, title, description, horizontal, information, details, allowC);
    }

    private static int ShowSelectionMenu(List<string> options, string title, string description, bool horizontal, string information, List<string> details, bool allowC)
    {
        if (options == null || options.Count == 0)
        {
            return -1;
        }

        int index = 0;

        title = $"  EL DORADO GRAND RESORT  |  {title}";

        if (description != null)
        {
            description = $"  • {description}";
        }

        while (true)
        {
            Console.Clear();

            ShowMenuHeader(title, description, horizontal, allowC);
            ShowMenuInformation(information);

            if (horizontal)
            {
                ShowHorizontalOptions(options, index, details);
            }
            else
            {
                ShowVerticalOptions(options, index, details);
            }

            ConsoleKey choice = Console.ReadKey(true).Key;

            if (choice == ConsoleKey.Escape)
            {
                return -1;
            }
            else if (choice == ConsoleKey.C && allowC)
            {
                return -2;
            }
            else if ((!horizontal && (choice == ConsoleKey.S || choice == ConsoleKey.DownArrow)) ||
                     (horizontal && (choice == ConsoleKey.D || choice == ConsoleKey.RightArrow)))
            {
                index++;

                if (index >= options.Count)
                {
                    index = 0;
                }
            }
            else if ((!horizontal && (choice == ConsoleKey.W || choice == ConsoleKey.UpArrow)) ||
                     (horizontal && (choice == ConsoleKey.A || choice == ConsoleKey.LeftArrow)))
            {
                index--;

                if (index < 0)
                {
                    index = options.Count - 1;
                }
            }
            else if (choice == ConsoleKey.Enter)
            {
                return index;
            }
        }
    }

    private static void ShowMenuHeader(string title, string description, bool horizontal, bool allowC)
    {
        Console.WriteLine("============================================================");
        Console.WriteLine(title.PadLeft((60 + title.Length) / 2));

        if (description != null)
        {
            Console.WriteLine(description.PadLeft((60 + description.Length) / 2));
        }

        Console.WriteLine("============================================================");
        Console.WriteLine();

        if (horizontal)
        {
            if (allowC)
            {
                Console.WriteLine("  ESC: Back          C: Confirm          ← →: Navigate          Enter: Select");
            }
            else
            {
                Console.WriteLine("  ESC: Back          ← →: Navigate          Enter: Select");
            }
        }
        else
        {
            if (allowC)
            {
                Console.WriteLine("  ESC: Back          C: Confirm          ↑ ↓: Navigate          Enter: Select");
            }
            else
            {
                Console.WriteLine("  ESC: Back          ↑ ↓: Navigate          Enter: Select");
            }
        }

        Console.WriteLine();
    }

    private static void ShowMenuInformation(string information)
    {
        if (information != null)
        {
            Console.WriteLine(information);
        }
    }

    private static void ShowVerticalOptions(List<string> options, int index, List<string> details)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (index == i)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"       >  {options[i]}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"          {options[i]}");
            }

            if (details != null && i < details.Count)
            {
                Console.WriteLine($"             {details[i]}");
            }
        }
    }

    private static void ShowHorizontalOptions(List<string> options, int index, List<string> details)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (index == i)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" > {options[i]}");
                Console.ResetColor();
            }
            else
            {
                Console.Write($"   {options[i]}");
            }

            if (i < options.Count - 1)
            {
                Console.Write("  |");
            }
        }

        Console.WriteLine();

        if (details != null)
        {
            for (int i = 0; i < details.Count; i++)
            {
                Console.Write($"   {details[i]}");

                if (i < details.Count - 1)
                {
                    Console.Write("  |");
                }
            }

            Console.WriteLine();
        }
    }

}