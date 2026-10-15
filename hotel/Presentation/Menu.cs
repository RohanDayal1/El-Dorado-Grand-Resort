using Dapper;
using Microsoft.Data.Sqlite;

static class Menu
{
    public static void Start()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the El Dorado Grand Resort Management System");
            Console.WriteLine("Enter 1 to test the database connection");
            Console.WriteLine("Enter 0 to exit");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Clear();
                    TestDatabaseConnection();
                    ConsoleHelper.Pause();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice");
                    ConsoleHelper.Pause();
                    break;
            }
        }
    }

    private static void TestDatabaseConnection()
    {
        using var connection = DatabaseInitializer.Connect();
        int count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Reservation");
        Console.WriteLine($"Database connected! Reservations: {count}");
    }
}
