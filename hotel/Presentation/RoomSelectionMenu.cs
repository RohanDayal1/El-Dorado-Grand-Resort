public static class RoomSelectionMenu
{
    public static RoomSelectionModel? SelectRooms(DateTime checkInDate, DateTime checkOutDate,
        int adults, int children, string? preferredBedType)
    {
        var roomsLogic = new RoomsLogic();
        var reservationLogic = new ReservationLogic();
        var selection = new RoomSelectionModel();

        selection.AvailableRooms = roomsLogic.GetAvailableRooms(checkInDate, checkOutDate);
        selection.RoomTypes = roomsLogic.GetAvailableRoomTypes(
            selection.AvailableRooms, selection.SelectedRooms, preferredBedType);

        if (selection.AvailableRooms.Count == 0)
        {
            Console.WriteLine("  No rooms are available for these dates. Please try another stay.");
            ConsoleHelper.WaitForEnter();
            return null;
        }

        while (true)
        {
            ConsoleHelper.ShowHeader("SELECT YOUR ROOMS");
            Console.WriteLine($"  Stay: {checkInDate:yyyy-MM-dd} to {checkOutDate:yyyy-MM-dd}");
            var dataErrors = reservationLogic.ValidateSelectedRoomData(
                selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);
            if (dataErrors.Count > 0)
            {
                ShowSelectionErrors(dataErrors);
                ConsoleHelper.WaitForEnter();
                return null;
            }

            var selectedTypes = reservationLogic.GetSelectedRoomTypes(
                selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);
            var capacity = reservationLogic.CalculateTotalCapacity(selectedTypes);
            var cost = reservationLogic.CalculateTotalCost(checkInDate, checkOutDate, selectedTypes);
            Console.WriteLine($"\n  Party: {adults} adults, {children} children");
            string action = ReadSelectionAction(selection.SelectedRooms, selection.AvailableRooms,
                selection.RoomTypes, capacity, cost);

            switch (action)
            {
                case "1":
                    if (!AddRooms(selection, preferredBedType)) return null;
                    break;
                case "2":
                    RemoveSelectedRoom(selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);
                    break;
                case "3":
                    var errors = reservationLogic.ValidateRoomSelection(adults, children,
                        selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);
                    if (errors.Count == 0) return selection;
                    ShowSelectionErrors(errors);
                    Console.Write("\n  Press Enter to continue selecting rooms...");
                    Console.ReadLine();
                    break;
                case "q":
                    return null;
                default:
                    Console.WriteLine("  Choose 1, 2, 3, or q.");
                    Console.Write("\n  Press Enter to continue selecting rooms...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static bool AddRooms(RoomSelectionModel selection, string? preferredBedType)
    {
        var logic = new RoomsLogic();
        var choices = logic.GetAvailableRoomTypes(
            selection.AvailableRooms, selection.SelectedRooms, preferredBedType);
        if (choices.Count == 0)
        {
            Console.WriteLine("  All available rooms are selected. Remove a room or continue.");
            Console.Write("\n  Press Enter to continue selecting rooms...");
            Console.ReadLine();
            return true;
        }

        ConsoleHelper.ShowHeader("ADD ROOMS");
        ShowAvailableRoomTypes(choices, selection.AvailableRooms, selection.SelectedRooms);
        RoomTypeModel? type = ReadRoomType(choices);
        if (type == null) return false;
        var availableQuantity = logic.GetAvailableQuantity(type, selection.AvailableRooms, selection.SelectedRooms);
        int? quantity = ReadRoomQuantity(type, availableQuantity);
        if (quantity == null) return false;

        var result = logic.SelectRooms(type, quantity.Value, selection.AvailableRooms, selection.SelectedRooms);
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

    private static string? ReadSelectionAction(List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms, List<RoomTypeModel> roomTypes,
        int totalCapacity, decimal totalCost)
    {
        Console.WriteLine("\n  SELECTED ROOMS");
        if (selectedRooms.Count == 0)
            Console.WriteLine("  No rooms selected.");
        foreach (var type in roomTypes)
        {
            var quantity = selectedRooms.Count(selected => availableRooms.Any(room =>
                room.RoomId == selected.RoomId && room.Room_Type_Id == type.Room_Type_Id));
            if (quantity > 0)
                Console.WriteLine($"  {quantity} x {type.Name}");
        }
        Console.WriteLine($"  Rooms: {selectedRooms.Count} | Combined capacity: {totalCapacity} | Total: {totalCost:C}");
        Console.WriteLine("  [1] Add rooms  [2] Remove a room  [3] Continue  [q] Cancel");
        return ReservationInput.ReadText("Select an option: ");
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
        for (int index = 0; index < selectedRooms.Count; index++)
        {
            var room = availableRooms.First(room => room.RoomId == selectedRooms[index].RoomId);
            var type = roomTypes.First(type => type.Room_Type_Id == room.Room_Type_Id);
            Console.WriteLine($"  [{index + 1}] {type.Name}");
        }
        while (true)
        {
            int? choice = ReservationInput.ReadInt("Choose a room to remove (q to go back): ");
            if (choice == null) return false;
            if (choice >= 1 && choice <= selectedRooms.Count)
            {
                selectedRooms.RemoveAt(choice.Value - 1);
                return true;
            }
            Console.WriteLine("  Choose one of the selected rooms.");
        }
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
