public static class AdminMenu
{
    private const string Border = "============================================================";

    public static void StartAdminMenu()
    {
        bool viewadminmenu = true;
        while (viewadminmenu)
        {
            ConsoleHelper.ClearScreen();
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
                    ShowReservationDetails();
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
                    break;
            }
        if (viewadminmenu)
        {
            Console.Write("  Press Enter to return to the admin menu...");
            Console.ReadLine();
        }
        }
    }

    public static void ShowAdminMenu()
    {
        ConsoleHelper.ClearScreen();
		Console.WriteLine(Border);
        Console.WriteLine("  EL DORADO GRAND RESORT  |  ADMIN MENU");
        Console.WriteLine(Border);
        Console.WriteLine();

        Console.WriteLine("[1] View all registered guests");
        Console.WriteLine("[2] View all reservations");
        Console.WriteLine("[3] View existing reservation");
        Console.WriteLine("[4] Edit an existing reservation");
        Console.WriteLine("[5] Cancel an existing reservation");
        Console.WriteLine("[0] Back");
    }


    public static void ShowAllGuests()
    {
        ConsoleHelper.ClearScreen();
        var allGuests = new AdminLogic().GetAllGuests();
        foreach (var guest in allGuests)
        {
            Console.WriteLine($"  User ID:         {guest.UserId}");
            Console.WriteLine($"  Name:            {guest.FirstName} {guest.LastName}");
            Console.WriteLine($"  Email:           {guest.Email}");
            Console.WriteLine($"  Phone:           {guest.Phone}");
            Console.WriteLine($"  Date of birth:   {guest.DateOfBirth:yyyy-MM-dd}");
            Console.WriteLine();
        }
    }

    public static void ShowAllReservation()
    {
        ConsoleHelper.ClearScreen();
        var allReservation = new AdminLogic().ShowAllReservation();
        foreach (var reservation in allReservation)
        {
            Console.WriteLine($"  Reservation ID:  {reservation.ReservationId}");
            Console.WriteLine($"  Code:            {reservation.ReservationCode}");
            Console.WriteLine($"  Guest:           {reservation.FirstName} {reservation.LastName}");
            Console.WriteLine($"  Check-in:        {reservation.CheckInDate:yyyy-MM-dd}");
            Console.WriteLine($"  Check-out:       {reservation.CheckOutDate:yyyy-MM-dd}");
            Console.WriteLine($"  Adults:          {reservation.Adults}");
            Console.WriteLine($"  Children:        {reservation.Children}");
            Console.WriteLine($"  Total:           {reservation.TotalCost:C}");
            Console.WriteLine($"  Status:          {reservation.Status}");
            Console.WriteLine();
        }
    }

    public static void ShowReservationDetails()
    {
        ConsoleHelper.ClearScreen();
        Console.Write("Reservation ID: ");
        string? id = Console.ReadLine();
        if (!int.TryParse(id, out int rID))
        {
            Console.WriteLine("invalid id");
            return;
        }
        ReservationModel? reservation = new AdminLogic().GetReservation(rID);
        if (reservation == null)
        {
            Console.WriteLine("Reservation doesn't exist");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"  Reservation ID:  {reservation.ReservationId}");
        Console.WriteLine($"  Code:            {reservation.ReservationCode}");
        Console.WriteLine($"  User ID:         {reservation.UserId}");
        Console.WriteLine($"  Check-in:        {reservation.CheckInDate:yyyy-MM-dd}");
        Console.WriteLine($"  Check-out:       {reservation.CheckOutDate:yyyy-MM-dd}");
        Console.WriteLine($"  Adults:          {reservation.Adults}");
        Console.WriteLine($"  Children:        {reservation.Children}");
        Console.WriteLine($"  Total:           {reservation.TotalCost:C}");
        Console.WriteLine($"  Status:          {reservation.Status}");
    }

    public static void ShowEditReservation()
    {
        ConsoleHelper.ClearScreen();
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
        ConsoleHelper.ClearScreen();
        int? reservation = ReservationInput.ReadInt("Reservation ID: ");
        if (reservation == null)
        {
            return;
        }

        string result = new AdminLogic().CancelReservation(reservation.Value);
        Console.WriteLine(result);
    }

}