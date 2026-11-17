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

    
    public static int SelectionMenu(List<string> options, string title = "", string description = null)
    {
        if (options is null || options.Count == 0)
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

            Console.WriteLine("============================================================");
            Console.WriteLine(title.PadLeft((60 + title.Length) / 2));
            
            if (description != null)
            {
                Console.WriteLine(description.PadLeft((60 + description.Length) / 2));
            }

            Console.WriteLine("============================================================");
            Console.WriteLine();
            Console.WriteLine("  ESC: Back          ↑ ↓: Navigate          Enter: Select");
            Console.WriteLine();

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
            }

            ConsoleKey choice = Console.ReadKey(true).Key;

            if (choice == ConsoleKey.Escape)
            {
                return -1;
            }
            else if (choice == ConsoleKey.S || choice == ConsoleKey.DownArrow)
            {
                index++;

                if (index >= options.Count)
                {
                    index = 0;
                }
            }
            else if (choice == ConsoleKey.W || choice == ConsoleKey.UpArrow)
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

    public static int SelectionMenu(List<string> options, string title, string description, bool horizontal)
    {
        if (options is null || options.Count == 0)
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

            Console.WriteLine("============================================================");
            Console.WriteLine(title.PadLeft((60 + title.Length) / 2));

            if (description != null)
            {
                Console.WriteLine(description.PadLeft((60 + description.Length) / 2));
            }

            Console.WriteLine("============================================================");
            Console.WriteLine();
            Console.WriteLine("  ESC: Back          ← →: Navigate          Enter: Select");
            Console.WriteLine();

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

            ConsoleKey choice = Console.ReadKey(true).Key;

            if (choice == ConsoleKey.Escape)
            {
                return -1;
            }
            else if (choice == ConsoleKey.D || choice == ConsoleKey.RightArrow)
            {
                index++;

                if (index >= options.Count)
                {
                    index = 0;
                }
            }
            else if (choice == ConsoleKey.A || choice == ConsoleKey.LeftArrow)
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

}
