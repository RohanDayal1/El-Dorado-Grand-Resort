using Microsoft.VisualBasic;

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
                case "6":
                    StartRoomMenu();
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
            Console.Write("Press Enter to return to the admin menu...");
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
        Console.WriteLine("[6] Open Room Menu");
        Console.WriteLine("[0] Back");
    }

    public static void StartRoomMenu()
    {
        bool viewroommenu = true;
        while (viewroommenu)
        {
            ConsoleHelper.ClearScreen();
            ShowRoomMenu();
            Console.Write("Select an option: ");
            string? choice = Console.ReadLine()?.Trim();
			if (choice == null)
				return;
            
            switch (choice)
            {
                case "1":
                    //ShowAllRooms();
                    int test = AdminAccess.CreateRoom();
                    Console.WriteLine(test);
                    break;
                case "2":
                    NewRoomType();
                    break;
                case "0":
                    viewroommenu = false;
                    break;
                default:
                    Console.WriteLine("invalid option");
                    break;
            }
            if (viewroommenu)
            {
                Console.Write("  Press Enter to return to the room menu...");
                Console.ReadLine();
            }
        }
    }

    public static void ShowRoomMenu()
    {
        ConsoleHelper.ClearScreen();
		Console.WriteLine(Border);
        Console.WriteLine("  EL DORADO GRAND RESORT  |  ROOM MENU");
        Console.WriteLine(Border);
        Console.WriteLine();

        Console.WriteLine("[1] View all rooms");
        Console.WriteLine("[2] Add new room type");
        Console.WriteLine("[0] Back to Admin Menu");
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

    public static void ShowAllRooms()
    {
        List<RoomDetailModel> Rooms = new AdminLogic().GetAllRooms();
        foreach (var room in Rooms)
        {
            Console.WriteLine();
            Console.WriteLine($"  Room ID:         {room.RoomId}");
            Console.WriteLine($"  RoomTypeID:      {room.Room_Type_Id}");
            Console.WriteLine($"  Room Number:     {room.RoomNumber}");
            Console.WriteLine($"  Room Name:       {room.Name}");
            Console.WriteLine($"  Capacity:        {room.Capacity}");
            Console.WriteLine($"  Price Per Night: {room.PricePerNight}");
            Console.WriteLine($"  BedType:         {room.BedType}");

            Console.WriteLine();
        }
    }

    public static void NewRoomType()
    {
        while (true)
        {
            ConsoleHelper.ClearScreen();
            Console.Write("RoomType Name: ");
            string? RoomTypeName = Console.ReadLine()?.Trim() ?? string.Empty; 
            Console.Write("Description: ");
            string? RoomTypeDescription = Console.ReadLine()?.Trim() ?? string.Empty;
            int Capacity = 0;
            decimal PricePerNight = 0;
            while (true)
            {
                Console.Write("Capacity: ");
                string? inputCapacity = Console.ReadLine()?.Trim();
                if (!int.TryParse(inputCapacity, out Capacity))
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }
                break;
            }
            while (true)
            {
                Console.Write("Price per night: ");
                string? inputPrice = Console.ReadLine()?.Trim();
                if (!decimal.TryParse(inputPrice, out PricePerNight))
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }
                break;
            }
            Console.Write("BedType: ");
            string? BedType = Console.ReadLine()?.Trim() ?? string.Empty;
            
            var logic = new AdminLogic();
            var errors = logic.ValidateRoomType(RoomTypeName, RoomTypeDescription, Capacity, PricePerNight, BedType);

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    string errorMessage = error switch
                                                {
                                                    RoomTypeErrors.InvalidName => "Room type name is required. Please enter a name.",
                                                    RoomTypeErrors.InvalidDescription => "Description is required. Please enter a short description.",
                                                    RoomTypeErrors.InvalidCapacity => "Capacity must be at least 1 guest and no more than 10.",
                                                    RoomTypeErrors.InvalidPricePerNight => "Price per night must be more than 0.",
                                                    RoomTypeErrors.InvalidBedType => "Bed type is required. Please enter one, like (double) or (twin).",
                                                    _ => "Please check your input and try again."
                                                };
                    Console.WriteLine(errorMessage);
                }
                ConsoleHelper.Pause();
                continue;
            }

            RoomTypeModel roomtype = new RoomTypeModel 
                                    {
                                        Name = RoomTypeName, 
                                        Description = RoomTypeDescription, 
                                        Capacity = Capacity, 
                                        PricePerNight = PricePerNight, 
                                        BedType = BedType
                                    };
            if (!ShowRoomTypeConfirmation(roomtype))
            {
                Console.WriteLine("No new roomtype got added");
                break;
            }
            logic.AddRoomType(roomtype);
            Console.WriteLine("New roomtype added");
            break;
        }
    }

    public static bool ShowRoomTypeConfirmation(RoomTypeModel roomtype)
    {
        Console.WriteLine();
        Console.WriteLine("CONFIRM NEW ROOMTYPE");
        Console.WriteLine($" Name:             {roomtype.Name}");
        Console.WriteLine($" Description:      {roomtype.Description}");
        Console.WriteLine($" Capacity:         {roomtype.Capacity}");
        Console.WriteLine($" Price Per Night:  {roomtype.PricePerNight}");
        Console.WriteLine($" Bed Type:         {roomtype.BedType}");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("Confirm these details? (y/n)");
            string? choice = Console.ReadLine();
            if (choice == "n")
            {
                return false;
            }
            else if (choice == "y")
            {
                return true;
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }
    }

    public static string? ReadInput(string message)
    {
        Console.WriteLine($"{message}");
        string? input = Console.ReadLine()?.Trim().ToLower();
        if (input == null || input == "q") {return null;}
        return input;
    }
}