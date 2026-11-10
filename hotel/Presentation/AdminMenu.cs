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
                    ShowAllRooms();
                    break;
                case "2":
                    NewHotelRoom();
                    break;
                case "3":
                    NewRoomType();
                    break;
                case "4":
                    ShowEditRoomTypeOfRoom();
                    break;
                case "5":
                    ShowEditRoomTypeInformation();
                    //test();
                    break;
                case "6":
                    ShowRemoveRoom();
                    break;
                case "0":
                    viewroommenu = false;
                    break;
                default:
                    Console.WriteLine("invalid option");
                    continue;
            }
            if (viewroommenu)
            {
                Console.Write("Press Enter to return to the room menu...");
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
        Console.WriteLine("[2] Add new room");
        Console.WriteLine("[3] Add new room type");
        Console.WriteLine("[4] Edit a room's type");
        Console.WriteLine("[5] Edit a roomtype information's");
        Console.WriteLine("[6] Remove a room");
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
        string? rCode = ReadInput("Reservation Code: ");
        if (rCode == null)
        {
            return;
        }

        ReservationModel? reservation = new AdminLogic().GetReservation(rCode);
        if (reservation == null)
        {
            Console.WriteLine("Reservation doesn't exist");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"  Reservation ID:  {reservation.ReservationId}");
        Console.WriteLine($"  Code:            {reservation.ReservationCode}");
        Console.WriteLine($"  Guest:           {reservation.FirstName} {reservation.LastName}");
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
        string? rCode = ReadInput("Reservation Code: ");
        if (rCode == null)
        {
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
        
        string result = new AdminLogic().EditReservation(rCode, newCheckInDate, newCheckOutDate);
        Console.WriteLine(result);
    }

    public static void ShowCancelReservation()
    {
        ConsoleHelper.ClearScreen();
        string? rCode = ReadInput("Reservation Code: ");
        if (rCode == null)
        {
            return;
        }

        string result = new AdminLogic().CancelReservation(rCode);
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

    public static void NewHotelRoom()
    {
        while (true)
        {
            ConsoleHelper.ClearScreen();
            int roomNumber = ReadInputInt("Room number (101-120, 201-220, 301-320, 401-420): ");

            var logic = new AdminLogic();
            List<RoomTypeModel> allRoomTypes = logic.GetAllRoomTypes();
            if (allRoomTypes.Count == 0)
            {
                Console.WriteLine("No room types exist yet. Add a room type first.");
                ConsoleHelper.Pause();
                return;
            }

            Console.WriteLine("Choose a room type");
            foreach (var roomtype in allRoomTypes)
            {
                Console.WriteLine($"[{roomtype.Room_Type_Id}] {roomtype.Name}");
            }
            int roomTypeId = ReadInputInt("Option: ");

            List<int> allExistingRoomNumbers = logic.GetExistingRooms();

            var errors = RoomValidator.ValidateRoom(roomNumber, allRoomTypes, roomTypeId, allExistingRoomNumbers);
            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    string errorMessage = error switch
                                                {
                                                    RoomErrors.InvalidFloor => "The floor must be 1 to 4. For example: 105 is on floor 1.",
                                                    RoomErrors.InvalidRoomNumber => "The room on that floor must be 01 to 20.",
                                                    RoomErrors.InvalidRoomType => "Choose a valid room type.",
                                                    RoomErrors.RoomNumberTaken => $"Room {roomNumber} is already in use.",
                                                    _ => "Please check your input and try again."
                                                };
                    Console.WriteLine(errorMessage);
                }
                ConsoleHelper.Pause();
                continue;
            }

            RoomModel room = new RoomModel{RoomNumber = roomNumber, Room_Type_Id = roomTypeId};
            string? roomTypeName = allRoomTypes.FirstOrDefault(roomtype => roomtype.Room_Type_Id == room.Room_Type_Id)?.Name;

            List<string> lines = new()
            {
                $" RoomNumber:     {room.RoomNumber}",
                $" RoomType:       {room.Room_Type_Id}",
                $" RoomType Name:  {roomTypeName}"
            };

            if (!ShowConfirmation(lines))
            {
                Console.WriteLine("No new room added");
                break;
            }
            logic.AddRoom(room);
            Console.WriteLine("Room added");
            break;
        }
    }

    public static void NewRoomType()
    {
        while (true)
        {
            ConsoleHelper.ClearScreen();
            string? RoomTypeName = ReadInput("Roomtype Name: ");
            if (RoomTypeName == null)
            {
                return;
            }

            string? RoomTypeDescription = ReadInput("Description: ");
            if (RoomTypeDescription == null)
            {
                return;
            }

            int Capacity = ReadInputInt("Capacity: ");

            decimal PricePerNight;
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

            string? BedType = ReadInput("Bedtype: ");
            if (BedType == null)
            {
                return;
            }
            
            RoomTypeModel roomtype = new RoomTypeModel 
                                    {
                                        Name = RoomTypeName, 
                                        Description = RoomTypeDescription, 
                                        Capacity = Capacity, 
                                        PricePerNight = PricePerNight, 
                                        BedType = BedType
                                    };
            var errors = RoomTypeValidator.ValidateRoomType(roomtype);

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

            List<string> lines = new()
            {
                $" Name:             {roomtype.Name}",
                $" Description:      {roomtype.Description}",
                $" Capacity:         {roomtype.Capacity}",
                $" Price Per Night:  {roomtype.PricePerNight}",
                $" Bed Type:         {roomtype.BedType}"
            };

            if (!ShowConfirmation(lines))
            {
                Console.WriteLine("No new roomtype got added");
                break;
            }
            new AdminLogic().AddRoomType(roomtype);
            Console.WriteLine("New roomtype added");
            break;
        }
    }

    public static void ShowEditRoomTypeOfRoom()
    {
        while (true)
        {
            ConsoleHelper.ClearScreen();
            int roomNumber = ReadInputInt("Room number: ");

            var logic = new AdminLogic();
            List<RoomErrors> errors = logic.ValidateRemoveRoom(roomNumber);
            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    string errorMessage = error switch
                                                {
                                                    RoomErrors.RoomNotFound => "No room with that room number exists",
                                                    RoomErrors.RoomHasReservations => "This room has confirmed reservations, so it can't be edited.",
                                                    _ => "Please check your input and try again."
                                                };
                    Console.WriteLine(errorMessage);
                }
                ConsoleHelper.Pause();
                continue;
            }

            List<RoomTypeModel> allRoomTypes = logic.GetAllRoomTypes();
            if (allRoomTypes.Count == 0)
            {
                Console.WriteLine("No room types exist yet. Add a room type first.");
                ConsoleHelper.Pause();
                return;
            }

            Console.WriteLine("Choose a room type");
            foreach (var roomtype in allRoomTypes)
            {
                Console.WriteLine($"[{roomtype.Room_Type_Id}] {roomtype.Name}");
            }
            int roomTypeId = ReadInputInt("Option: ");

            bool result = RoomValidator.ValidateRoomTypeId(allRoomTypes, roomTypeId);
            if (!result)
            {
                Console.WriteLine("Choose a valid room type.");
                continue;
            }

            List<RoomDetailModel> allRooms = logic.GetAllRooms();
            string? oldRoomTypeName = allRooms.FirstOrDefault(room => room.RoomNumber == roomNumber)?.Name;
            string? newRoomTypeName = allRoomTypes.FirstOrDefault(roomtype => roomtype.Room_Type_Id == roomTypeId)?.Name;

            List<string> lines = new() 
            {
                $"Confirm changes to room {roomNumber}:",
                $"{oldRoomTypeName} -> {newRoomTypeName}"
            };

            if (!ShowConfirmation(lines))
            {
                Console.WriteLine("Changes abandoned");
                break;
            }

            logic.EditRoomTypeOfRoom(roomNumber, roomTypeId);
            Console.WriteLine("Changes applied");
            break;
        }
    }

    public static void ShowEditRoomTypeInformation()
    {
        while (true)
        {
            ConsoleHelper.ClearScreen();
            int roomTypeId = ReadInputInt("Roomtype id to edit: ");

            var logic = new AdminLogic();
            List<RoomTypeModel> allRoomTypes = logic.GetAllRoomTypes();

            var roomtypeidErrors = RoomValidator.ValidateRoomTypeId(allRoomTypes, roomTypeId);
            if (!roomtypeidErrors)
            {
                Console.WriteLine("Not a valid roomtype id.");
                ConsoleHelper.Pause();
                continue;
            }

            string? newRoomTypeName = ReadInput("New roomtype name: ");
            if (newRoomTypeName == null)
            {
                return;
            }

            string? NewRoomTypeDescription = ReadInput("New description: ");
            if (NewRoomTypeDescription == null)
            {
                return;
            }

            int newCapacity = ReadInputInt("New capacity: ");

            decimal newPricePerNight;
            while (true)
            {
                Console.Write("New price per night: ");
                string? inputPrice = Console.ReadLine()?.Trim();
                if (!decimal.TryParse(inputPrice, out newPricePerNight))
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }
                break;
            }

            string? newBedType = ReadInput("New bedtype: ");
            if (newBedType == null)
            {
                return;
            }

            RoomTypeModel roomtype = new RoomTypeModel 
                                    {   Room_Type_Id = roomTypeId,
                                        Name = newRoomTypeName, 
                                        Description = NewRoomTypeDescription, 
                                        Capacity = newCapacity, 
                                        PricePerNight = newPricePerNight, 
                                        BedType = newBedType
                                    };
            var errors = RoomTypeValidator.ValidateRoomType(roomtype);
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

            List<string> lines = new()
            {
                $"Confirm changes to roomtype id {roomTypeId}:",
                $" New Name:             {roomtype.Name}",
                $" New Description:      {roomtype.Description}",
                $" New Capacity:         {roomtype.Capacity}",
                $" New Price Per Night:  {roomtype.PricePerNight}",
                $" New Bed Type:         {roomtype.BedType}"
            };

            if (!ShowConfirmation(lines))
            {
                Console.WriteLine("Changes abandoned.");
            }
            logic.EditRoomTypeInformation(roomtype);
            Console.WriteLine("Changes applied");
            break;
        }
    }

    public static void ShowRemoveRoom()
    {
        while (true)
        {
            ConsoleHelper.ClearScreen();
            int roomNumber = ReadInputInt("Room number: ");

            var logic = new AdminLogic();
            List<RoomErrors> errors = logic.ValidateRemoveRoom(roomNumber);
            if (errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    string errorMessage = error switch
                                                {
                                                    RoomErrors.RoomNotFound => "No room with that room number exists",
                                                    RoomErrors.RoomHasReservations => "This room has confirmed reservations, so it can't be removed.",
                                                    _ => "Please check your input and try again."
                                                };
                    Console.WriteLine(errorMessage);
                }
                ConsoleHelper.Pause();
                continue;
            }
            
            List<RoomDetailModel> rooms = logic.GetAllRooms();
            RoomDetailModel? room = rooms.First(roomdetail => roomdetail.RoomNumber == roomNumber);
            List<string> lines = new()
            {
                "Room to remove",
                $"Room Id:        {room.RoomId}",
                $"Room Number:    {room.RoomNumber}",
                $"Roomtype Id:    {room.Room_Type_Id}",
                $"Roomtype Name:  {room.Name}"
            };

            if (!ShowConfirmation(lines))
            {
                Console.WriteLine("No room got removed");
                break;
            }
            logic.RemoveRoom(roomNumber);
            Console.WriteLine("Room removed");
            break;
        }
    }

    public static string? ReadInput(string message)
    {
        while (true)
        {
            Console.Write($"{message}");
            string? input = Console.ReadLine()?.Trim();
            if (input == null || input == "q" || input == "Q") {return null;}
            return input;
        }
    }

    public static int ReadInputInt(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine()?.Trim();
            if (!int.TryParse(input, out int intOutput))
            {
                Console.WriteLine("Invalid input");
                continue;
            }
            return intOutput;
        }
    }

    public static bool ShowConfirmation(List<string> lines)
    {
        Console.WriteLine();
        Console.WriteLine("CONFIRM DETAILS");
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        Console.WriteLine();

        while (true)
        {
            Console.Write("Confirm these details? (y/n): ");
            string? choice = Console.ReadLine()?.Trim().ToLower();
            if (choice == "y")
                return true;
            if (choice == "n")
                return false;
            Console.WriteLine("Invalid input");
        }
    }
}