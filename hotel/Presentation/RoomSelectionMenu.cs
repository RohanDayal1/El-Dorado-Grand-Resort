public static class RoomSelectionMenu
{
    public static RoomSelectionModel? SelectRooms(DateTime checkInDate, DateTime checkOutDate,
        int adults, int children, string? preferredBedType, int? reservationId = null, List<ReservationRoomModel>? selectedRooms = null)
    {
        RoomsLogic roomsLogic = new RoomsLogic();
        ReservationLogic reservationLogic = new ReservationLogic();
        RoomSelectionModel selection = new RoomSelectionModel();

        if (selectedRooms != null)
            selection.SelectedRooms = selectedRooms;

        selection.AvailableRooms = roomsLogic.GetAvailableRooms(checkInDate, checkOutDate, reservationId);

        if (reservationId == null)
        {
            selection.RoomTypes = roomsLogic.GetAvailableRoomTypes(selection.AvailableRooms, selection.SelectedRooms, preferredBedType);
        }
        else
        {
            selection.RoomTypes = roomsLogic.GetRoomTypes();
        }

        if (selection.AvailableRooms.Count == 0)
        {
            Console.WriteLine("  No rooms are available for these dates. Please try another stay.");
            ConsoleHelper.WaitForEnter();
            return null;
        }

        while (true)
        {
            List<RoomSelectionErrors> dataErrors = reservationLogic.ValidateSelectedRoomData(selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);

            if (dataErrors.Count > 0)
            {
                ShowSelectionErrors(dataErrors);
                ConsoleHelper.WaitForEnter();
                return null;
            }

            List<RoomTypeModel> selectedTypes = reservationLogic.GetSelectedRoomTypes(selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);

            int capacity = reservationLogic.CalculateTotalCapacity(selectedTypes);
            decimal cost = reservationLogic.CalculateTotalCost(checkInDate, checkOutDate, selectedTypes);

            int action = ReadSelectionAction(selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes, capacity, cost, checkInDate, checkOutDate, adults, children);

            switch (action)
            {
                case 0:
                    if (!AddRooms(selection, preferredBedType)) return null;
                    break;

                case 1:
                    RemoveSelectedRoom(selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);
                    break;

                case 2:
                    List<RoomSelectionErrors> errors = reservationLogic.ValidateRoomSelection(adults, children, selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);

                    if (errors.Count == 0) return selection;

                    ShowSelectionErrors(errors);
                    Console.Write("\n  Press Enter to continue selecting rooms...");
                    Console.ReadLine();
                    break;

                case 3:
                case -1:
                    return null;
            }
        }
    }

    private static bool AddRooms(RoomSelectionModel selection, string? preferredBedType)
    {
        var logic = new RoomsLogic();
        var choices = logic.GetAvailableRoomTypes(selection.AvailableRooms, selection.SelectedRooms, preferredBedType);

        if (choices.Count == 0)
        {
            Console.WriteLine("  All available rooms are selected. Remove a room or continue.");
            Console.Write("\n  Press Enter to continue selecting rooms...");
            Console.ReadLine();
            return true;
        }

        ConsoleHelper.ShowHeader("ADD ROOMS");
        List<string> options = new List<string>();

        foreach (RoomTypeModel type in choices)
        {
            int roomQuantity = logic.GetAvailableQuantity(type, selection.AvailableRooms, selection.SelectedRooms);
            string option = $"{type.Name} | Capacity: {type.Capacity} | Beds: {type.BedType} | Per night: {type.PricePerNight:C} | Available: {roomQuantity}";

            if (!string.IsNullOrWhiteSpace(type.Description))
                option += $" | {type.Description}";

            options.Add(option);
        }

        int choice = ConsoleHelper.SelectionMenu(options, "ADD ROOMS", "Choose a room type");

        if (choice == -1)
            return true;

        RoomTypeModel selectedType = choices[choice];

        int availableQuantity = logic.GetAvailableQuantity(selectedType, selection.AvailableRooms, selection.SelectedRooms);

        int? quantity = ReadRoomQuantity(selectedType, availableQuantity);

        if (quantity == null) return false;

        var result = logic.SelectRooms(selectedType, quantity.Value, selection.AvailableRooms, selection.SelectedRooms);

        if (result.Errors.Count > 0)
        {
            ShowQuantityErrors(result.Errors);
            Console.Write("\n  Press Enter to continue selecting rooms...");
            Console.ReadLine();
        }
        else
            selection.SelectedRooms.AddRange(result.Rooms);

        return true;
    }

    private static void ShowAvailableRoomTypes(List<RoomTypeModel> roomTypes,
        List<RoomModel> availableRooms, List<ReservationRoomModel> selectedRooms)
    {
        var logic = new RoomsLogic();
        Console.WriteLine("\n  AVAILABLE ROOM TYPES");

        for (int index = 0; index < roomTypes.Count; index++)
        {
            var type = roomTypes[index];
            int quantity = logic.GetAvailableQuantity(type, availableRooms, selectedRooms);
            Console.WriteLine($"  [{index + 1}] {type.Name} | Capacity: {type.Capacity} | Beds: {type.BedType}");
            Console.WriteLine($"      Per night: {type.PricePerNight:C} | Available: {quantity}");

            if (!string.IsNullOrWhiteSpace(type.Description))
                Console.WriteLine($"      {type.Description}");
        }
    }

    private static RoomTypeModel? ReadRoomType(List<RoomTypeModel> roomTypes)
    {
        while (true)
        {
            int? choice = ReservationInput.ReadInt("Choose a room type: ");

            if (choice == null) return null;

            if (choice >= 1 && choice <= roomTypes.Count)
                return roomTypes[choice.Value - 1];

            Console.WriteLine("  Choose one of the listed room types.");
        }
    }

    private static int? ReadRoomQuantity(RoomTypeModel roomType, int availableQuantity)
    {
        var logic = new ReservationLogic();

        while (true)
        {
            int? quantity = ReservationInput.ReadInt($"How many {roomType.Name} rooms? ");

            if (quantity == null) return null;

            var errors = logic.ValidateRoomQuantity(quantity.Value, availableQuantity);

            if (errors.Count == 0)
                return quantity;

            ShowQuantityErrors(errors);
        }
    }

    private static int ReadSelectionAction(List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms, List<RoomTypeModel> roomTypes,
        int totalCapacity, decimal totalCost, DateTime checkInDate, DateTime checkOutDate, int adults, int children)
    {
        List<string> options = new List<string>
        {
            "Add rooms",
            "Remove a room",
            "Continue",
            "Cancel"
        };

        int index = 0;

        while (true)
        {
            ConsoleHelper.ClearScreen();

            Console.WriteLine("============================================================");
            Console.WriteLine("  EL DORADO GRAND RESORT  |  SELECT YOUR ROOMS");
            Console.WriteLine("============================================================");
            Console.WriteLine($"  Stay: {checkInDate:yyyy-MM-dd} to {checkOutDate:yyyy-MM-dd}");
            Console.WriteLine($"  Party: {adults} adults, {children} children");

            Console.WriteLine("\n  SELECTED ROOMS");

            if (selectedRooms.Count == 0)
                Console.WriteLine("  No rooms selected.");

            foreach (var type in roomTypes)
            {
                var quantity = selectedRooms.Count(selected => availableRooms.Any(room => room.RoomId == selected.RoomId && room.Room_Type_Id == type.Room_Type_Id));

                if (quantity > 0)
                {
                    Console.WriteLine($"  {quantity} x {type.Name} | Capacity: {type.Capacity} | Beds: {type.BedType}");
                    Console.WriteLine($"      Per night: {type.PricePerNight:C}");

                    if (!string.IsNullOrWhiteSpace(type.Description))
                        Console.WriteLine($"      {type.Description}");
                }
            }

            Console.WriteLine($"\n  Rooms: {selectedRooms.Count} | Combined capacity: {totalCapacity} | Total: {totalCost:C}");
            Console.WriteLine("\n  ESC: Back          A/D or ← →: Navigate          Enter: Select");
            Console.WriteLine();

            for (int i = 0; i < options.Count; i++)
            {
                if (index == i)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($" > {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write($"   {options[i]}");
                }

                if (i < options.Count - 1)
                    Console.Write("  |");
            }
            Console.WriteLine();

            ConsoleKey choice = Console.ReadKey(true).Key;

            if (choice == ConsoleKey.Escape)
                return -1;
            else if (choice == ConsoleKey.D || choice == ConsoleKey.RightArrow)
            {
                index++;

                if (index >= options.Count)
                    index = 0;
            }
            else if (choice == ConsoleKey.A || choice == ConsoleKey.LeftArrow)
            {
                index--;

                if (index < 0)
                    index = options.Count - 1;
            }
            else if (choice == ConsoleKey.Enter)
                return index;
        }
    }

    private static bool RemoveSelectedRoom(List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms, List<RoomTypeModel> roomTypes)
    {
        if (selectedRooms.Count == 0)
        {
            Console.WriteLine("  There are no rooms to remove.");
            Console.Write("\n  Press Enter to continue selecting rooms...");
            Console.ReadLine();
            return false;
        }

        ConsoleHelper.ShowHeader("REMOVE A ROOM");

        List<string> options = new List<string>();

        foreach (var selectedRoom in selectedRooms)
        {
            var room = availableRooms.First(room => room.RoomId == selectedRoom.RoomId);
            var type = roomTypes.First(type => type.Room_Type_Id == room.Room_Type_Id);
            options.Add(type.Name);
        }

        int choice = ConsoleHelper.SelectionMenu(options, "REMOVE A ROOM", "Choose a room to remove");

        if (choice == -1)
            return false;

        selectedRooms.RemoveAt(choice);
        return true;
    }

    public static void ShowSelectedRooms(List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms, List<RoomTypeModel> roomTypes,
        DateTime checkInDate, DateTime checkOutDate)
    {
        var logic = new ReservationLogic();
        var errors = logic.ValidateSelectedRoomData(selectedRooms, availableRooms, roomTypes);

        if (errors.Count > 0)
        {
            ShowSelectionErrors(errors);
            return;
        }

        var selectedTypes = logic.GetSelectedRoomTypes(selectedRooms, availableRooms, roomTypes);

        foreach (var type in roomTypes)
        {
            var roomsOfType = new List<RoomTypeModel>();

            foreach (var selectedType in selectedTypes)
            {
                if (selectedType.Room_Type_Id == type.Room_Type_Id)
                    roomsOfType.Add(selectedType);
            }

            if (roomsOfType.Count == 0) continue;

            decimal cost = logic.CalculateTotalCost(checkInDate, checkOutDate, roomsOfType);
            decimal roomCost = logic.CalculateRoomCost(type, checkInDate, checkOutDate);

            Console.WriteLine($"  {roomsOfType.Count} x {type.Name} | Per room/night: {type.PricePerNight:C}");
            Console.WriteLine($"      Per room/stay: {roomCost:C} | All {type.Name} rooms: {cost:C}");
        }
    }

    private static void ShowQuantityErrors(List<RoomQuantityErrors> errors)
    {
        foreach (var error in errors)
            Console.WriteLine($"  {ReservationMenu.GetRoomQuantityMessage(error)}");
    }

    private static void ShowSelectionErrors(List<RoomSelectionErrors> errors)
    {
        foreach (var error in errors)
            Console.WriteLine($"  {ReservationMenu.GetRoomSelectionMessage(error)}");
    }
}
