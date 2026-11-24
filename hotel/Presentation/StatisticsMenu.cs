public static class StatisticsMenu
{
    private const string Border = "============================================================";

    public static void Start(AccountModel? account)
    {
        ConsoleHelper.ClearScreen();

        if (!new AdminLogic().IsAdmin(account))
        {
            Console.WriteLine("Access denied. Resort statistics are only available to administrators.");
            return;
        }

        ResortStatisticsModel statistics = new StatisticsLogic().GetResortStatistics();

        Console.WriteLine(Border);
        Console.WriteLine("  EL DORADO GRAND RESORT  |  STATISTICS");
        Console.WriteLine(Border);
        Console.WriteLine();
        Console.WriteLine($"  Total reservations:       {statistics.TotalReservations}");
        Console.WriteLine($"  Active reservations:      {statistics.ActiveReservations}");
        Console.WriteLine($"  Cancelled reservations:   {statistics.CancelledReservations}");
        Console.WriteLine($"  Registered guests:        {statistics.RegisteredGuests}");
    }
}
