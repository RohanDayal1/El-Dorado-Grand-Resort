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
                    ShowAllGuests();
                    break;
                case "2":
                    ShowAllReservation();
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
                    ShowEditReservation();
                    break;
                case "5":
                    ShowCancelReservation();
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


    public static void ShowAllGuests()
    {
        var allGuests = new AdminLogic().GetAllGuests();
        foreach (var guest in allGuests)
        {
            Console.WriteLine(@$"Userid: {guest.UserId}, 
            First name: {guest.FirstName}, 
            Last name: {guest.LastName}, 
            Email: {guest.Email}, 
            Phone: {guest.Phone}, 
            Date of birth: {guest.DateOfBirth:yyyy-MM-dd}");
        }
    }

    public static void ShowAllReservation()
    {
        var allReservation = AdminAccess.GetAllReservation();
        foreach (var reservation in allReservation)
        {
            Console.WriteLine(@$"Reservationid: {reservation.ReservationId}, 
            Reservationcode: {reservation.ReservationCode}, 
            UserId: {reservation.UserId}, 
            CheckInDate: {reservation.CheckInDate:yyyy-MM-dd}, 
            CheckOutDate: {reservation.CheckOutDate:yyyy-MM-dd}, 
            TotalCost: {reservation.TotalCost}, 
            Status: {reservation.Status}");
        }
    }

    public static void ShowEditReservation()
    {
        Console.Write("Reservation ID: ");
        string? id = Console.ReadLine();

        if (!int.TryParse(id, out int reservationId))
        {
            Console.WriteLine("not a valid number!");
            return;
        }

        Console.Write("New Check-in-Date: ");
        string? checkInDate = Console.ReadLine();
        if (!DateTime.TryParse(checkInDate, out DateTime newCheckInDate))
        {
            Console.WriteLine("not a valid date");
            return;
        }

        Console.Write("New Check-Out-Date: ");
        string? checkOutDate = Console.ReadLine();
        if (!DateTime.TryParse(checkOutDate, out DateTime newCheckOutDate))
        {
            Console.WriteLine("not a valid date");
            return;
        }
        
        string result = new AdminLogic().EditReservation(reservationId, newCheckInDate, newCheckOutDate);
        Console.WriteLine(result);
    }

    public static void ShowCancelReservation()
    {
        int? reservation = ReservationInput.ReadInt("Reservation ID: ");
        if (reservation == null)
        {
            return;
        }

        string result = new AdminLogic().CancelReservation(reservation.Value);
        Console.WriteLine(result);
    }

}