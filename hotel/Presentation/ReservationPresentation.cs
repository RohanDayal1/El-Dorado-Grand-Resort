public static class ReservationPresentation
{
    private const string Border = "============================================================";

    public static void ShowReservations(AccountModel account)
    {
        List<ReservationModel> reservations = new ReservationLogic()
            .GetByUserId(account.UserId)
            .ToList();

        List<FacilityReservationModel> facilityBookings =
            new FacilityReservationLogic().GetByUserId(account.UserId);

        if (reservations.Count == 0)
        {
            ConsoleHelper.ShowHeader("YOUR RESERVATIONS");
            Console.WriteLine("No reservations were found for your account.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        while (true)
        {
            List<string> options = new();

            int counter = 1;

            foreach (ReservationModel reservation in reservations)
            {
                options.Add($"[{counter}] |  {reservation.ReservationCode}, {reservation.CheckInDate} - {reservation.CheckOutDate} | {reservation.Status}");
                counter++;
            }

            int num = ConsoleHelper.SelectionMenu(options, $"RESERVATIONS FOR {(account.FirstName + " " + account.LastName).ToUpper()}", "Select a reservation to view its details");

            if (num == -1 || num == -2)
                return;

            ReservationModel selectedReservation = reservations[num];

            string reservationDetails =
                $"    Reservation code: {selectedReservation.ReservationCode}\n" +
                $"    Check-in:         {selectedReservation.CheckInDate:yyyy-MM-dd}\n" +
                $"    Check-out:        {selectedReservation.CheckOutDate:yyyy-MM-dd}\n" +
                $"    Status:           {selectedReservation.Status}\n" +
                $"    Length of stay:   {(selectedReservation.CheckOutDate - selectedReservation.CheckInDate).Days} nights\n" +
                $"    Adults:           {selectedReservation.Adults}\n" +
                $"    Children:         {selectedReservation.Children}\n" +
                $"    Total:            {selectedReservation.TotalCost}\n";

            List<FacilityReservationModel> bookingsForStay = facilityBookings
                .Where(booking => booking.ReservationId == selectedReservation.ReservationId)
                .ToList();

            if (bookingsForStay.Count > 0)
            {
                reservationDetails += "\n    FACILITY BOOKINGS:\n";

                foreach (FacilityReservationModel booking in bookingsForStay)
                {
                    reservationDetails += FacilityReservationMenu.GetBookingDetails(booking);
                }
            }

            ReviewModel? review = new ReviewLogic().GetByReservationId(selectedReservation.ReservationId);
            int action = ConsoleHelper.SelectionMenu(new List<string>
            {
                "Modify reservation",
                "Cancel reservation",
                review == null ? "Leave a review" : "View my review"
            }, "RESERVATION DETAILS", "Choose what you want to do", null, reservationDetails);

            if (action == -1)
                continue;

            if (action == -2)
                return;

            if (action == 0)
            {
                ModifyReservation(selectedReservation);
            }
            else if (action == 1)
            {
                ReservationCancellationMenu.CancelReservation(selectedReservation);

                ReservationLogic reservationLogic = new();
                reservations = reservationLogic.GetByUserId(account.UserId).ToList();

                if (reservations.Count == 0)
                {
                    Console.Clear();
                    ConsoleHelper.ShowHeader("YOUR RESERVATIONS");
                    Console.WriteLine("You have no reservations.");
                    ConsoleHelper.WaitForEnter();
                    return;
                }
            }
            else if (action == 2)
            {
                ReviewMenu.Start(selectedReservation);
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
