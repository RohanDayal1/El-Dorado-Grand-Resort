
public static class ReservationCancellationMenu
{
    private const string Border = "============================================================";

    public static void CancelReservation(ReservationModel reservation)
    {
        ReservationLogic logic = new ReservationLogic();

        ConsoleHelper.ClearScreen();
        Console.WriteLine(Border);
        Console.WriteLine("  EL DORADO GRAND RESORT  |  CANCEL RESERVATION");
        Console.WriteLine(Border);
        Console.WriteLine();

        Console.WriteLine($"  Reservation code: {reservation.ReservationCode}");
        Console.WriteLine($"  Check-in:         {reservation.CheckInDate:yyyy-MM-dd}");
        Console.WriteLine($"  Check-out:        {reservation.CheckOutDate:yyyy-MM-dd}");
        Console.WriteLine($"  Total cost:       {reservation.TotalCost}");
        Console.WriteLine($"  Status:           {reservation.Status}");

        Console.WriteLine();
        Console.WriteLine(Border);

        if (!logic.CanCancelReservation(reservation))
        {
            Console.WriteLine();
            Console.WriteLine("  Cancellation is no longer possible.");
            Console.WriteLine("  Reservations can only be cancelled at least");
            Console.WriteLine("  48 hours before the scheduled arrival.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        Console.WriteLine();
        Console.WriteLine("  You can still cancel this reservation.");
        Console.WriteLine();

        string? choice = ReservationInput.ReadText(
            "Are you sure you want to cancel? (y/n): "
        );

        if (choice?.ToLower() != "y")
        {
            Console.WriteLine();
            Console.WriteLine("  Cancellation process stopped.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        bool cancelled = logic.CancelReservation(reservation.ReservationId);

        Console.WriteLine();

        if (cancelled)
        {
            Console.WriteLine("  Your reservation has been cancelled.");
        }
        else
        {
            Console.WriteLine("  The reservation could not be cancelled.");
            Console.WriteLine("  Please check the cancellation deadline.");
        }

        ConsoleHelper.WaitForEnter();
    }
}
