public static class Menu
{
	private const string Border = "============================================================";

	public static void Start()
	{
		bool running = true;
		AccountModel? signedInAccount = null;

		while (running)
		{
			ShowHomePage(signedInAccount);

			string? choice = Console.ReadLine()?.Trim();

			switch (choice)
			{
				case "1":
					ShowPage("ABOUT US", new[]
					{
						"Welcome to El Dorado Grand Resort.",
						"Our resort is a relaxing destination for guests",
						"looking for comfort, warm hospitality and a",
						"memorable holiday.",
						"",
						"Our team is here to make every stay special."
					});
					break;
				case "2":
					ShowPage("OUR FACILITIES", new[]
					{
						"Enjoy everything you need for a comfortable stay:",
						"  - Comfortable guest rooms",
						"  - Swimming pool",
						"  - Restaurant",
						"  - Wellness facilities"
					});
					break;
				case "3":
					ShowPage("CONTACT US", new[]
					{
						"Our team will be happy to help you.",
						"",
						"Email:   info@eldoradograndresort.com",
						"Resort:  El Dorado Grand Resort",
                        "Address:  123 Paradise Road, Beach City, Country",
                        "Phone:   +1 (555) 123-4567"
					});
					break;
				case "4":
					if (signedInAccount == null)
					{
						signedInAccount = Login();
						if (signedInAccount != null)
						{
							ShowMemberHome(signedInAccount);
						}
					}
					else
					{
						ShowMemberHome(signedInAccount);
					}
					break;
				case "5":
					if (signedInAccount == null)
					{
						Register();
					}
					else
					{
						Console.WriteLine($"\n  You have been logged out, {signedInAccount.FullName}.");
						signedInAccount = null;
					}
					break;
				case "0":
					Console.WriteLine("\nThank you for visiting El Dorado Grand Resort. See you soon!");
					running = false;
					break;
				default:
					Console.WriteLine("\nInvalid choice. Please choose one of the options shown.");
					break;
			}
		}
	}

	private static void ShowHomePage(AccountModel? signedInAccount)
	{
		Console.Clear();
		Console.WriteLine(Border);
		Console.WriteLine("                 EL DORADO GRAND RESORT");
		Console.WriteLine("                 Your comfort, our pleasure");
		Console.WriteLine(Border);
		Console.WriteLine();
		Console.WriteLine(signedInAccount == null
			? "  Welcome! What would you like to know?"
			: $"  Welcome back, {signedInAccount.FullName}!");
		Console.WriteLine();
		Console.WriteLine("  [1] About us");
		Console.WriteLine("  [2] Our facilities");
		Console.WriteLine("  [3] Contact us");
		Console.WriteLine(signedInAccount == null ? "  [4] Login" : "  [4] My home");
		Console.WriteLine(signedInAccount == null ? "  [5] Register" : "  [5] Log out");
		Console.WriteLine("  [0] Exit");
		Console.WriteLine();
		Console.Write("  Select an option: ");
	}

	private static void ShowPage(string title, string[] lines)
	{
		Console.Clear();
		Console.WriteLine(Border);
		Console.WriteLine($"  EL DORADO GRAND RESORT  |  {title}");
		Console.WriteLine(Border);
		Console.WriteLine();

		foreach (string line in lines)
		{
			Console.WriteLine($"  {line}");
		}

		Console.WriteLine();
		Console.WriteLine(Border);
		Console.Write("  Press Enter to return to the home menu...");
		Console.ReadLine();
	}

	private static AccountModel? Login()
	{
		Console.Clear();
		Console.WriteLine(Border);
		Console.WriteLine("  EL DORADO GRAND RESORT  |  LOGIN");
		Console.WriteLine(Border);
		Console.WriteLine();
		Console.Write("  Email address: ");
		string email = Console.ReadLine()?.Trim() ?? string.Empty;
		Console.Write("  Password: ");
		string password = Console.ReadLine() ?? string.Empty;
		Console.WriteLine();

		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			Console.WriteLine("  Please enter both your email address and password.");
			ReturnPrompt();
			return null;
		}

		AccountModel? account = new AccountsLogic().CheckLogin(email, password);
		if (account == null)
		{
			Console.WriteLine("  Login failed. No account was found with that email and password.");
			ReturnPrompt();
			return null;
		}

		return account;
	}

	private static void ShowMemberHome(AccountModel account)
	{
		Console.Clear();
		Console.WriteLine(Border);
		Console.WriteLine("  EL DORADO GRAND RESORT  |  YOUR HOME");
		Console.WriteLine(Border);
		Console.WriteLine();
		Console.WriteLine($"  You are now logged in as {account.FullName}.");
		Console.WriteLine($"  Account email: {account.EmailAddress}");
		Console.WriteLine();
		Console.WriteLine("  Welcome to your resort homepage!");
		Console.WriteLine("  Discover our comfortable rooms, swimming pool,");
		Console.WriteLine("  restaurant and wellness facilities.");
		Console.WriteLine();
		Console.WriteLine(Border);
		Console.Write("  Press Enter to continue to the home menu...");
		Console.ReadLine();
	}

	private static void Register()
	{
		Console.Clear();
		Console.WriteLine(Border);
		Console.WriteLine("  EL DORADO GRAND RESORT  |  REGISTER");
		Console.WriteLine(Border);
		Console.WriteLine();
		Console.Write("  Full name: ");
		string fullName = Console.ReadLine()?.Trim() ?? string.Empty;
		Console.Write("  Email address: ");
		string email = Console.ReadLine()?.Trim() ?? string.Empty;
		Console.Write("  Password: ");
		string password = Console.ReadLine() ?? string.Empty;
		Console.WriteLine();

		if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			Console.WriteLine("  Please fill in all fields to try to register.");
		}
		else
		{
			Console.WriteLine($"  Thanks, {fullName}. Your details were entered, but no account was saved.");
			Console.WriteLine("  Registration is not connected to an account system yet.");
		}

		ReturnPrompt();
	}

	private static void ReturnPrompt()
	{
		Console.WriteLine();
		Console.WriteLine(Border);
		Console.Write("  Press Enter to return to the home menu...");
		Console.ReadLine();
	}
}
