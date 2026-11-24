public static class StatisticsMenu
{
    public static void Start(AccountModel? account)
    {
        if (!new AdminLogic().IsAdmin(account))
        {
            ConsoleHelper.ShowHeader("ACCESS DENIED");
            Console.WriteLine("Access denied. Resort statistics are only available to administrators.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        var logic = new StatisticsLogic();
        while (true)
        {
            ResortStatisticsModel statistics = logic.GetResortStatistics();
            int choice = ConsoleHelper.SelectionMenu(
                new List<string> { "Refresh statistics", "Back to admin dashboard" },
                "STATISTICS",
                information: FormatStatistics(statistics)
            );

            if (choice != 0)
                return;
        }
    }

    private static string FormatStatistics(ResortStatisticsModel statistics)
    {
        var lines = new List<string>
        {
            $"  TODAY ({DateTime.Today:yyyy-MM-dd}) - OCCUPANCY",
            $"  Occupied rooms: {statistics.OccupiedRoomsToday}",
            $"  Available rooms: {statistics.AvailableRoomsToday}",
            $"  Occupancy: {statistics.OccupancyPercentage:F1}% ({statistics.OccupiedRoomsToday}/{statistics.ActiveRooms} active rooms)",
            $"  Guests staying: {statistics.GuestsStayingToday}",
            $"  Guests arriving: {statistics.ArrivingGuestsToday}",
            $"  Guests departing: {statistics.DepartingGuestsToday}",
            "",
            "  ALL-TIME - RESERVATIONS AND BOOKING VALUE",
            $"  Total reservations: {statistics.TotalReservations}",
            $"  Confirmed reservations: {statistics.ConfirmedReservations}",
            $"  Cancelled reservations: {statistics.CancelledReservations}",
            $"  Cancellation rate: {statistics.CancellationPercentage:F1}%",
            $"  Registered guest accounts: {statistics.RegisteredGuests}",
            $"  Confirmed booking value: {statistics.ConfirmedBookingValue:C}",
            $"  Average confirmed booking value: {statistics.AverageConfirmedBookingValue:C}",
            "",
            "  ALL-TIME - GUEST SATISFACTION",
            statistics.TotalReviews == 0
                ? "  Average rating: No reviews yet"
                : $"  Average rating: {statistics.AverageReviewRating:F1}/5",
            $"  Total reviews: {statistics.TotalReviews}",
            "  Reviews per rating:"
        };

        foreach (RatingStatistics rating in statistics.ReviewsByRating)
            lines.Add($"    {rating.Rating}/5: {rating.ReviewCount}");

        lines.Add("");
        lines.Add("  ALL-TIME - FACILITIES");
        lines.Add($"  Confirmed facility bookings: {statistics.ConfirmedFacilityBookings}");
        lines.Add("  Confirmed bookings per facility:");
        if (statistics.BookingsByFacility.Count == 0)
            lines.Add("    No facilities found.");
        else
            lines.AddRange(
                statistics.BookingsByFacility.Select(
                    facility => $"    {facility.FacilityName}: {facility.ConfirmedBookings}"
                )
            );

        lines.Add(
            statistics.MostBookedServiceName == null
                ? "  Most booked service: No confirmed service bookings yet."
                : $"  Most booked service: {statistics.MostBookedServiceName} ({statistics.MostBookedServiceCount} bookings)"
        );

        return string.Join(Environment.NewLine, lines);
    }
}
