public static class ReservationPresentation
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

        List<FacilityReservationModel> facilityBookings =
            new FacilityReservationLogic().GetByUserId(account.UserId);

        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations were found for your account.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        string input = "";

        while (input.ToLower() != "q")
        {
            Console.Clear();

            int counter = 1;

            Console.WriteLine(Border);
            Console.WriteLine(
                $"  EL DORADO GRAND RESORT  |  RESERVATIONS FOR {(account.FirstName + " " + account.LastName).ToUpper()}"
            );
            Console.WriteLine(Border);
            Console.WriteLine();

            foreach (ReservationModel reservation in reservations)
            {
                Console.WriteLine(Border);
                Console.WriteLine($"[{counter}]");
                Console.WriteLine();
                Console.WriteLine($"  Reservation ID:   {reservation.ReservationId}");
                Console.WriteLine($"  Reservation code: {reservation.ReservationCode}");
                Console.WriteLine($"  Check-in:         {reservation.CheckInDate:yyyy-MM-dd}");
                Console.WriteLine($"  Check-out:        {reservation.CheckOutDate:yyyy-MM-dd}");
                Console.WriteLine($"  Status:           {reservation.Status}");
                Console.WriteLine($"  Total:            {reservation.TotalCost:C}");
                Console.WriteLine();

                List<FacilityReservationModel> bookingsForStay = facilityBookings
                    .Where(booking => booking.ReservationId == reservation.ReservationId)
                    .ToList();

                if (bookingsForStay.Count > 0)
                {
                    Console.WriteLine("  FACILITY BOOKINGS");

                    foreach (FacilityReservationModel booking in bookingsForStay)
                    {
                        FacilityReservationMenu.ShowBooking(booking);
                    }
                }

                counter++;
            }

            if (facilityBookings.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("  To modify or cancel facility bookings, please call reception");
                Console.WriteLine("  or visit reception during your stay.");
            }

            Console.WriteLine(Border);
            Console.WriteLine();
            Console.WriteLine("Choose a reservation to modify:");
            Console.WriteLine("Q to return");
            Console.WriteLine();

            input = Console.ReadLine() ?? "";

            if (input.ToLower() == "q")
                return;

            int num;

            if (int.TryParse(input, out num) && num >= 1 && num <= reservations.Count)
            {
                Console.WriteLine();
                Console.WriteLine("[1] Modify reservation");
                Console.WriteLine("[2] Cancel reservation");
                Console.WriteLine("[Q] Back");
                Console.WriteLine();

                string action = Console.ReadLine()?.Trim().ToLower();

                if (action == "1")
                {
                    ModifyReservation(reservations[num - 1]);
                }
                else if (action == "2")
                {
                    ReservationCancellationMenu.CancelReservation(reservations[num - 1]);
                    ReservationLogic reservationLogic = new();
                    reservations = reservationLogic.GetByUserId(account.UserId).ToList();

                    if (reservations.Count == 0)
                    {
                        Console.Clear();
                        ConsoleHelper.ShowHeader("YOUR RESERVATIONS");
                        Console.WriteLine("  You have no reservations.");
                        ConsoleHelper.WaitForEnter();
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid number");
                Console.WriteLine("  Press Enter to try again.");
                Console.ReadKey();
            }
        }
    }


    private static void ModifyReservation(ReservationModel reservation)
    {
        ReservationLogic logic = new ReservationLogic();

        Console.WriteLine($"  Current check-out: {reservation.CheckOutDate:yyyy-MM-dd}");
        DateTime? newCheckOutDate = ReservationInput.ReadDate("New check-out date: ");

        if (newCheckOutDate == null)
            return;

        if (!logic.CanShortenReservation(reservation, newCheckOutDate.Value))
        {
            Console.WriteLine("  You can only shorten your stay.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        List<ReservationRoomModel> currentRooms = logic.GetRooms(reservation.ReservationId);

        RoomSelectionModel? selection = RoomSelectionMenu.SelectRooms(reservation.CheckInDate,newCheckOutDate.Value,reservation.Adults,reservation.Children,null,reservation.ReservationId,currentRooms);

        if (selection == null)
            return;

        List<RoomTypeModel> selectedTypes = logic.GetSelectedRoomTypes(selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes);

        decimal newTotalCost = logic.CalculateTotalCost(reservation.CheckInDate, newCheckOutDate.Value, selectedTypes);

        ConsoleHelper.ShowHeader("REVIEW CHANGES");
        Console.WriteLine($"  New check-out: {newCheckOutDate.Value:yyyy-MM-dd}");
        Console.WriteLine($"  New total:     {newTotalCost:C}");

        RoomSelectionMenu.ShowSelectedRooms(selection.SelectedRooms, selection.AvailableRooms, selection.RoomTypes, reservation.CheckInDate, newCheckOutDate.Value);

        string? confirm = ReservationInput.ReadText("Save these changes? (y/n): ");

        if (confirm?.ToLower() != "y")
        {
            Console.WriteLine("  Changes cancelled.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        reservation.CheckOutDate = newCheckOutDate.Value;

        logic.UpdateReservation(reservation,selection.SelectedRooms,selection.AvailableRooms,selection.RoomTypes);

        Console.WriteLine("  Reservation successfully updated.");

        ConsoleHelper.WaitForEnter();
    }

}