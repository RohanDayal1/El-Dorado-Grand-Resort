public static class RegisterMenu
{
    public static void Register()
    {
        UserModel accountUser = new();
		bool validating = true;

		while (validating)
		{
			Console.Clear();

			Console.Write("  First name: ");
			string firstName = Console.ReadLine()?.Trim() ?? string.Empty;
			Console.Write("  Last name: ");
			string lastName = Console.ReadLine()?.Trim() ?? string.Empty;
			Console.Write("  Email address: ");
			string email = Console.ReadLine()?.Trim() ?? string.Empty;
			Console.Write("  Phone number: ");
			string phoneNumber = Console.ReadLine()?.Trim() ?? string.Empty;
			Console.Write("  Birth date (yyyy-mm-dd): ");
			string birthDate = Console.ReadLine()?.Trim() ?? string.Empty;

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
			
				Console.Clear();
				Console.WriteLine("\nRegistration failed. The following issues occurred:");
				foreach (string e in validation)
				{
					Console.WriteLine(e);
				}
			}

			if (validating)
			{
				Console.WriteLine();
				Console.WriteLine("  Press any key to try again...");
				Console.ReadKey();
			}
		}

		bool registeringPassword = true;

		while (registeringPassword)
		{
			Console.Clear();
			Console.WriteLine("Password requirements:");
			Console.WriteLine("- 8 to 20 characters long");
			Console.WriteLine("- At least 1 uppercase character.");
			Console.WriteLine("- At least 1 lowercase character.");
			Console.WriteLine("- At least 1 digit");
			Console.WriteLine("- At least 1 special character\n\n");

			Console.Write("  Password: ");
			string password = Console.ReadLine() ?? string.Empty;
			Console.Write("  Repeat Password: ");
			string repeatPassword = Console.ReadLine() ?? string.Empty;
			Console.WriteLine();

			List<string> validation = AccountsLogic.Register(accountUser.UserId, password, repeatPassword, "guest");


			if (validation.Count == 0)
			{
				Console.Clear();
				Console.WriteLine("\n  Account succesfully created.");
				registeringPassword = false;
			}

			foreach(string line in validation)
			{
				Console.Clear();
				Console.WriteLine("The following issues occurred:");
				Console.WriteLine(line);
			}

			if (registeringPassword)
			{
				Console.WriteLine();
				Console.WriteLine("  Press any key to try again...");
				Console.ReadKey();
				Console.WriteLine();
				Console.Clear();
			}
		}
    }
}