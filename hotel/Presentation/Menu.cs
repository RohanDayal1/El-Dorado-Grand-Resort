static class Menu
{

    //This shows the menu. You can call back to this method to show the menu again
    //after another presentation method is completed.
    //You could edit this to show different menus depending on the user's role
    public static void Start()
    {
        while (true)
        {
            Console.WriteLine("Enter 1 to login");
            Console.WriteLine("Enter 2 to update a book");

            string input = Console.ReadLine()!;

            switch (input)
            {
                case "1":
                    UserLogin.Start();
                    break;
                case "2":
                    Console.WriteLine("Updating books is not yet implemented");
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}
