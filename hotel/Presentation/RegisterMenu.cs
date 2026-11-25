public static class RegisterMenu
{
    public static void Register()
    {
        UserModel accountUser = new();
        bool validating = true;

        while (validating)
        {
            Console.Clear();
			ConsoleHelper.ShowHeader("REGISTRATION");
            Console.WriteLine("  Press ESC at any time to cancel registration.\n");

            Console.Write("  First name: ");
            if (!TryReadInput(out string firstName)) return;

            Console.Write("  Last name: ");
            if (!TryReadInput(out string lastName)) return;

            Console.Write("  Email address: ");
            if (!TryReadInput(out string email)) return;

            Console.Write("  Phone number: ");
            if (!TryReadInput(out string phoneNumber)) return;

            Console.Write("  Birth date (yyyy-mm-dd): ");
            if (!TryReadInput(out string birthDate)) return;

            List<string> validation = UserValidator.Register(firstName, lastName, birthDate, email, phoneNumber);

            if (validation.Count == 0)
            {
                UserModel? createdUser = UserLogic.GetByEmail(email);

                if (createdUser != null)
                {
                    accountUser = createdUser;
                    validating = false;
                }
            }
            else
            {
                ShowErrors(validation);
            }

            if (validating)
            {
                if (!TryAgain()) return;
            }
        }

        bool registeringPassword = true;

        while (registeringPassword)
        {
            Console.Clear();
            Console.WriteLine("  Press ESC at any time to cancel registration.\n");
            Console.WriteLine("Password requirements:");
            Console.WriteLine("- 8 to 20 characters long");
            Console.WriteLine("- At least 1 uppercase character.");
            Console.WriteLine("- At least 1 lowercase character.");
            Console.WriteLine("- At least 1 digit");
            Console.WriteLine("- At least 1 special character\n\n");

            Console.Write("  Password: ");
            if (!TryReadInput(out string password)) return;

            Console.Write("  Repeat Password: ");
            if (!TryReadInput(out string repeatPassword)) return;

            List<string> validation = AccountsLogic.Register(accountUser.UserId, password, repeatPassword, "guest");

            if (validation.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("\n  Account successfully created.");
                registeringPassword = false;
            }
            else
            {
                ShowErrors(validation);

                if (!TryAgain()) return;
            }
        }
    }

    private static bool TryReadInput(out string input)
    {
        input = string.Empty;

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\n\n  Registration cancelled.");
                return false;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return true;
            }
            else if (key.Key == ConsoleKey.Backspace)
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

    private static void ShowErrors(List<string> errors)
    {
        Console.Clear();
        Console.WriteLine("\nThe following issues occurred:");

        foreach (string error in errors)
        {
            Console.WriteLine(error);
        }
    }

    private static bool TryAgain()
    {
        Console.WriteLine();
        Console.WriteLine("  Press any key to try again or ESC to return to the main menu...");

        ConsoleKey key = Console.ReadKey(true).Key;

        return key != ConsoleKey.Escape;
    }
}