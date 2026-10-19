public static class ReservationMenu
{
    private const string Border = "============================================================";

    public static void ShowReservations(AccountModel account)
    {
        ConsoleHelper.ClearScreen();
        Console.WriteLine(Border);
        Console.WriteLine(
            $"  EL DORADO GRAND RESORT  |  RESERVATIONS FOR {(account.FirstName + " " + account.LastName).ToUpper()}"
        );
        Console.WriteLine(Border);
        Console.WriteLine();

        List<ReservationModel> reservations = new ReservationLogic()
            .GetByUserId(account.UserId)
            .ToList();

        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations were found for your account.");
        }
        else
        {
            foreach (ReservationModel reservation in reservations)
            {
                Console.WriteLine($"  Reservation ID:   {reservation.ReservationId}");
                Console.WriteLine($"  Reservation code: {reservation.ReservationCode}");
                Console.WriteLine($"  Check-in:         {reservation.CheckInDate:yyyy-MM-dd}");
                Console.WriteLine($"  Check-out:        {reservation.CheckOutDate:yyyy-MM-dd}");
                Console.WriteLine($"  Status:           {reservation.Status}");
                Console.WriteLine($"  Total:            {reservation.TotalCost:C}");
                Console.WriteLine();
            }
        }

        Console.WriteLine(Border);
        Console.Write("Press Enter to return to your home menu...");
        Console.ReadLine();
    }

    public static void Start(AccountModel? account = null)
    {
        ConsoleHelper.ShowHeader("MAKE A RESERVATION");
        DateTime today = DateTime.Today;
        var logic = new ReservationLogic();
        var roomsLogic = new RoomsLogic();

        var dates = ReadStayDates(today);
        if (dates == null)
            return;
        var counts = ReadGuestCounts();
        if (counts == null)
            return;

        RoomSelectionModel? selection;
        try
        {
            var types = roomsLogic.GetRoomTypes();
            string? preference = ReadBedPreference(types);
            if (preference == null)
                return;
            selection = RoomSelectionMenu.SelectRooms(
                dates.Value.CheckInDate,
                dates.Value.CheckOutDate,
                counts.Value.Adults,
                counts.Value.Children,
                preference
            );
        }
        catch (Exception)
        {
            Console.WriteLine(
                "Rooms could not be loaded. Please try again later. Nothing was booked."
            );
            ConsoleHelper.WaitForEnter();
            return;
        }
        if (selection == null)
            return;

        UserModel? user;
        if (account == null)
        {
            user = GuestDetailsMenu.ReadGuestDetails(today);
        }
        else
        {
            try
            {
                user = new AccountsLogic().GetGuestDetails(account);
            }
            catch (Exception)
            {
                user = null;
            }
            if (user == null)
            {
                Console.WriteLine(
                    "Your saved guest details could not be loaded. Please sign in again."
                );
                ConsoleHelper.WaitForEnter();
                return;
            }
        }
        if (user == null)
            return;

        var rooms = selection;
        var cost = logic.CalculateTotalCost(
            dates.Value.CheckInDate,
            dates.Value.CheckOutDate,
            rooms.SelectedRooms,
            rooms.AvailableRooms,
            rooms.RoomTypes
        );
        if (cost.Errors.Count > 0)
        {
            foreach (var error in cost.Errors)
                Console.WriteLine($"  {GetRoomSelectionMessage(error)}");
            ConsoleHelper.WaitForEnter();
            return;
        }

        var draft = new ReservationModel
        {
            CheckInDate = dates.Value.CheckInDate,
            CheckOutDate = dates.Value.CheckOutDate,
            Adults = counts.Value.Adults,
            Children = counts.Value.Children,
            TotalCost = cost.TotalCost,
        };
        if (!ShowReview(user, draft, rooms.SelectedRooms, rooms.AvailableRooms, rooms.RoomTypes))
        {
            Console.WriteLine("  Reservation abandoned. Nothing was booked.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        ReservationModel confirmed;
        try
        {
            confirmed =
                account == null
                    ? logic.ConfirmGuestReservation(
                        user,
                        draft.CheckInDate,
                        draft.CheckOutDate,
                        draft.Adults,
                        draft.Children,
                        rooms.SelectedRooms,
                        rooms.AvailableRooms,
                        today
                    )
                    : logic.ConfirmAccountReservation(
                        account,
                        draft.CheckInDate,
                        draft.CheckOutDate,
                        draft.Adults,
                        draft.Children,
                        rooms.SelectedRooms,
                        rooms.AvailableRooms,
                        today
                    );
        }
        catch (Exception)
        {
            Console.WriteLine("  The reservation could not be completed. Nothing was booked.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        ShowConfirmation(confirmed);
    }

    private static (DateTime CheckInDate, DateTime CheckOutDate)? ReadStayDates(DateTime today)
    {
        var logic = new ReservationLogic();
        while (true)
        {
            DateTime? checkIn = ReservationInput.ReadDate("Check-in (yyyy-MM-dd): ");
            if (checkIn == null)
                return null;
            DateTime? checkOut = ReservationInput.ReadDate("Check-out (yyyy-MM-dd): ");
            if (checkOut == null)
                return null;
            var errors = logic.ValidateDates(checkIn.Value, checkOut.Value, today);
            if (errors.Count == 0)
                return (checkIn.Value, checkOut.Value);
            ShowDateErrors(errors);
        }
    }

    private static (int Adults, int Children)? ReadGuestCounts()
    {
        ConsoleHelper.ShowHeader("NUMBER OF GUESTS");
        var logic = new ReservationLogic();
        while (true)
        {
            var adults = ReservationInput.ReadInt("Number of adults: ");
            if (adults == null)
                return null;
            var children = ReservationInput.ReadInt("Number of children: ");
            if (children == null)
                return null;
            var errors = logic.ValidateGuestCounts(adults.Value, children.Value);
            if (errors.Count == 0)
                return (adults.Value, children.Value);
            ShowGuestCountErrors(errors);
        }
    }

    private static string? ReadBedPreference(List<RoomTypeModel> roomTypes)
    {
        ConsoleHelper.ShowHeader("BED PREFERENCE");
        var beds = roomTypes
            .Where(type => !string.IsNullOrWhiteSpace(type.BedType))
            .Select(type => type.BedType!)
            .Distinct()
            .ToList();
        Console.WriteLine("\n  BED PREFERENCE (optional)");
        for (int index = 0; index < beds.Count; index++)
            Console.WriteLine($"  [{index + 1}] {beds[index]}");
        while (true)
        {
            string? input = ReservationInput.ReadText(
                "Choose a bed number, or press Enter for no preference: "
            );
            if (input == null)
                return null;
            if (input == "")
                return "";
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= beds.Count)
                return beds[choice - 1];
            Console.WriteLine("  Choose one of the listed options, or press Enter.");
        }
    }

    private static bool ShowReview(
        UserModel user,
        ReservationModel reservation,
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes
    )
    {
        ConsoleHelper.ShowHeader("REVIEW YOUR RESERVATION");
        Console.WriteLine($"  Guest:         {user.FirstName} {user.LastName}");
        Console.WriteLine($"  Email:         {user.Email}");
        Console.WriteLine($"  Phone:         {user.Phone}");
        Console.WriteLine($"  Date of birth: {user.DateOfBirth:yyyy-MM-dd}");
        Console.WriteLine($"  Check-in:      {reservation.CheckInDate:yyyy-MM-dd}");
        Console.WriteLine($"  Check-out:     {reservation.CheckOutDate:yyyy-MM-dd}");
        Console.WriteLine(
            $"  Nights:        {(reservation.CheckOutDate - reservation.CheckInDate).Days}"
        );
        Console.WriteLine(
            $"  Party:         {reservation.Adults} adults, {reservation.Children} children"
        );
        Console.WriteLine();
        RoomSelectionMenu.ShowSelectedRooms(
            selectedRooms,
            availableRooms,
            roomTypes,
            reservation.CheckInDate,
            reservation.CheckOutDate
        );
        Console.WriteLine($"\n  Total: {reservation.TotalCost:C}");
        while (true)
        {
            string? choice = ReservationInput.ReadText("Confirm reservation? (y/n): ");
            if (choice == null || choice.Equals("n", StringComparison.OrdinalIgnoreCase))
                return false;
            if (choice.Equals("y", StringComparison.OrdinalIgnoreCase))
                return true;
            Console.WriteLine("  Enter y to confirm, n to abandon, or q to cancel.");
        }
    }

    private static void ShowConfirmation(ReservationModel reservation)
    {
        ConsoleHelper.ClearScreen();
        Console.WriteLine(Border);
        Console.WriteLine("  EL DORADO GRAND RESORT  |  RESERVATION CONFIRMED");
        Console.WriteLine(Border);
        Console.WriteLine($"\n  Reservation code: {reservation.ReservationCode}");
        Console.WriteLine($"  Check-in:         {reservation.CheckInDate:yyyy-MM-dd}");
        Console.WriteLine($"  Check-out:        {reservation.CheckOutDate:yyyy-MM-dd}");
        Console.WriteLine($"  Total:            {reservation.TotalCost:C}");
        Console.WriteLine("  Keep your reservation code for reference.");
        ConsoleHelper.WaitForEnter();
    }

    private static void ShowDateErrors(List<ReservationDateErrors> errors)
    {
        foreach (var error in errors)
        {
            string message = error switch
            {
                ReservationDateErrors.CheckInDateInPast => "Check-in must be today or later.",
                ReservationDateErrors.CheckOutDateNotAfterCheckIn =>
                    "Check-out must be after check-in.",
                _ => "Please check your stay dates.",
            };
            Console.WriteLine($"  {message}");
        }
    }

    private static void ShowGuestCountErrors(List<GuestCountErrors> errors)
    {
        foreach (var error in errors)
        {
            string message = error switch
            {
                GuestCountErrors.NoAdults => "Enter at least one adult.",
                GuestCountErrors.NegativeChildren => "The number of children cannot be negative.",
                _ => "Please check the number of guests.",
            };
            Console.WriteLine($"  {message}");
        }
    }

    public static List<string> ValidateRoomQuantity(int quantity, int availableQuantity)
    {
        var service = new ReservationLogic();
        var errors = service.ValidateRoomQuantity(quantity, availableQuantity);
        var messages = new List<string>();

        foreach (var error in errors)
        {
            messages.Add(GetRoomQuantityMessage(error));
        }

        return messages;
    }

    public static bool CanContinueWithRooms(
        int adults,
        int children,
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes
    )
    {
        var service = new ReservationLogic();
        return service
                .ValidateRoomSelection(adults, children, selectedRooms, availableRooms, roomTypes)
                .Count == 0;
    }

    public static List<string> ValidateRoomSelection(
        int adults,
        int children,
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes
    )
    {
        var service = new ReservationLogic();
        var errors = service.ValidateRoomSelection(
            adults,
            children,
            selectedRooms,
            availableRooms,
            roomTypes
        );
        var messages = new List<string>();

        foreach (var error in errors)
        {
            messages.Add(GetRoomSelectionMessage(error));
        }

        return messages;
    }

    public static string GetRoomSelectionMessage(RoomSelectionErrors error) =>
        error switch
        {
            RoomSelectionErrors.NoRoomsSelected => "Select at least one room.",
            RoomSelectionErrors.MoreRoomsThanAdults =>
                "Each room needs an adult. Select no more rooms than adults.",
            RoomSelectionErrors.InsufficientCapacity =>
                "Select rooms with enough combined capacity for all adults and children.",
            RoomSelectionErrors.DuplicateRoom =>
                "A room was selected twice. Please change the selection.",
            RoomSelectionErrors.RoomUnavailable =>
                "A selected room is unavailable. Please start again.",
            RoomSelectionErrors.RoomTypeNotFound =>
                "A selected room type could not be found. Please contact reception.",
            _ => "The room selection could not be validated.",
        };

    public static string GetRoomQuantityMessage(RoomQuantityErrors error) =>
        error switch
        {
            RoomQuantityErrors.QuantityBelowOne => "Select at least one room.",
            RoomQuantityErrors.QuantityExceedsAvailability =>
                "There are not enough available rooms of this type.",
            _ => "The room quantity could not be validated.",
        };
}
