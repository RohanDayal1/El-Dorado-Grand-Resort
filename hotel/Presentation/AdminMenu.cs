public static class AdminMenu
{
    private const string Border = "============================================================";

    public static void StartAdminMenu()
    {
        bool viewadminmenu = true;
        while (viewadminmenu)
        {
            ShowAdminMenu();
            Console.Write("Select an option: ");
            string? choice = Console.ReadLine()?.Trim();
			if (choice == null)
				return;

            switch (choice)
            {
                case "1":
                    AdminAccess.ViewAllGuest();
                    break;
                case "2":
                    AdminAccess.ViewAllReservation();
                    break;
                case "3":
                    Console.Write("Reservation ID: ");
                    string? id = Console.ReadLine();
                    if (int.TryParse(id, out int rID))
                    {
                        ReservationModel? reservation = AdminAccess.GetReservation(rID);
                        if (reservation != null)
                        {
                            Console.WriteLine($"\n ReservationId: {reservation.ReservationId}");
                            Console.WriteLine($" Reservation Code: {reservation.ReservationCode}");
                            Console.WriteLine($" UserId: {reservation.UserId}");
                            Console.WriteLine($" Check in date {reservation.CheckInDate:yyyy-MM-dd}");
                            Console.WriteLine($" Check out date {reservation.CheckOutDate:yyyy-MM-dd}");
                            Console.WriteLine($" Total Cost: {reservation.TotalCost}");
                            Console.WriteLine($" Status: {reservation.Status}");
                        }
                        else
                        {
                            Console.WriteLine("Reservation doesn't exist");
                        }
                    }
                    else
                    {
                        Console.WriteLine("invalid id");
                    }
                    break;
                case "4":
                    AdminAccess.EditReservation();
                    break;
                case "5":
                    AdminAccess.CancelReservation();
                    break;
                case "0":
                    viewadminmenu = false;
                    break;
                default:
                    Console.WriteLine("invalid option");
                    continue;
            }
        }
    }

    public static void ShowAdminMenu()
    {
		Console.WriteLine(Border);
        Console.WriteLine("Welcome to the Admin Menu");
        Console.WriteLine(Border);
        Console.WriteLine();

        Console.WriteLine("[1] View all registered guests");
        Console.WriteLine("[2] View all reservations");
        Console.WriteLine("[3] Select existing reservation");
        Console.WriteLine("[4] Edit an existing reservation");
        Console.WriteLine("[5] Cancel an existing reservation");
        Console.WriteLine("[0] Back");
    }

}