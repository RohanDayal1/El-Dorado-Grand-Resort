public static class FacilityReservationMenu
{
    public static void Start(
        AccountModel account,
        FacilityModel facility,
        FacilityServiceModel service
    )
    {
        var bookingLogic = new FacilityReservationLogic();
        ConsoleHelper.ShowHeader("BOOK A FACILITY SERVICE");

        var hotelReservations = bookingLogic.GetFacilityReservations(account, DateTime.Now);

        if (hotelReservations.Count == 0)
        {
            Console.WriteLine("  You need a confirmed hotel stay to book a facility service.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        var hotelReservation = ChooseHotelReservation(hotelReservations);
        if (hotelReservation == null)
            return;

        var selectedSlot = ChooseAppointmentTime(account, hotelReservation, service);
        if (selectedSlot == null)
            return;

        int? numberOfPeople = NumberOfPeople(
            hotelReservation,
            selectedSlot.Value.RemainingCapacity
        );

        if (numberOfPeople == null)
            return;

        if (
            !CheckFacilityBooking(
                hotelReservation,
                facility,
                service,
                selectedSlot.Value.StartTime,
                numberOfPeople.Value
            )
        )
        {
            Console.WriteLine("  Booking abandoned. Nothing was booked.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        var bookingResult = bookingLogic.ConfirmReservation(
            account,
            hotelReservation.ReservationId,
            service.FacilityServiceId,
            selectedSlot.Value.StartTime,
            numberOfPeople.Value,
            DateTime.Now
        );

        if (bookingResult.Errors.Count > 0)
        {
            ShowValidationErrors(bookingResult.Errors);
            Console.WriteLine(
                "  Nothing was booked. Please select the service again to try again."
            );
            ConsoleHelper.WaitForEnter();
            return;
        }

        if (bookingResult.Reservation != null)
        {
            ConsoleHelper.ShowHeader("FACILITY BOOKING CONFIRMED");
            ShowBooking(bookingResult.Reservation);
            Console.WriteLine("  To modify or cancel this booking, please call reception");
            Console.WriteLine("  or visit reception during your stay.");
            ConsoleHelper.WaitForEnter();
        }
    }

    private static ReservationModel? ChooseHotelReservation(
        List<ReservationModel> hotelReservations
    )
    {
        if (hotelReservations.Count == 1)
            return hotelReservations[0];

        List<string> reservationNames = new List<string>();

        foreach (ReservationModel reservation in hotelReservations)
        {
            reservationNames.Add(
                $"{reservation.ReservationCode} | {reservation.CheckInDate:yyyy-MM-dd} to {reservation.CheckOutDate:yyyy-MM-dd}"
            );
        }

        int choice = ConsoleHelper.SelectionMenu(reservationNames, "SELECT YOUR HOTEL STAY", "Choose the stay for your facility booking");

        if (choice == -1)
            return null;

        return hotelReservations[choice];
    }

    private static (DateTime StartTime, int RemainingCapacity)? ChooseAppointmentTime(
        AccountModel account,
        ReservationModel hotelReservation,
        FacilityServiceModel service
    )
    {
        var bookingLogic = new FacilityReservationLogic();

        while (true)
        {
            ConsoleHelper.ShowHeader("SELECT YOUR APPOINTMENT");
            Console.WriteLine($"  Hotel reservation: {hotelReservation.ReservationCode}");
            Console.WriteLine($"  Stay: {hotelReservation.CheckInDate:yyyy-MM-dd} to {hotelReservation.CheckOutDate:yyyy-MM-dd}");
            Console.WriteLine("  Appointments must take place before the checkout date.");
            Console.WriteLine($"  Service: {service.Name} | Duration: {service.DurationMinutes} minutes\n");

            DateTime? appointmentDate = ReservationInput.ReadDate("Appointment date (yyyy-MM-dd): ");

            if (appointmentDate == null)
                return null;

            var availableSlots = bookingLogic.GetAvailableStartTimes(
                account,
                hotelReservation.ReservationId,
                service.FacilityServiceId,
                appointmentDate.Value,
                DateTime.Now
            );

            if (availableSlots.Count == 0)
            {
                Console.WriteLine("  No start times are available for this date.");
                Console.WriteLine("  Choose another date during your stay, before checkout.");
                continue;
            }

            List<string> availableTimes = new List<string>();

            foreach (var slot in availableSlots)
            {
                availableTimes.Add(slot.StartTime.ToString("HH:mm"));
            }

            int choice = ConsoleHelper.SelectionMenu(availableTimes, "AVAILABLE START TIMES", $"Choose an appointment time for {appointmentDate.Value:yyyy-MM-dd}");

            if (choice == -1)
                return null;

            return availableSlots[choice];
        }
    }

    private static int? NumberOfPeople(ReservationModel hotelReservation, int remainingCapacity)
    {
        var bookingLogic = new FacilityReservationLogic();
        ConsoleHelper.ShowHeader("NUMBER OF PEOPLE");

        Console.WriteLine(
            $"  Your stay: {hotelReservation.Adults} adults, {hotelReservation.Children} children"
        );
        Console.WriteLine($"  Places available: {remainingCapacity}");
        Console.WriteLine(
            "  Everyone in this booking receives the same service at the same time.\n"
        );

        while (true)
        {
            int? numberOfPeople = ReservationInput.ReadInt("Number of people: ");

            if (numberOfPeople == null)
                return null;

            var errors = bookingLogic.ValidatePartySize(
                numberOfPeople.Value,
                hotelReservation.Adults,
                hotelReservation.Children,
                remainingCapacity
            );

            if (errors.Count == 0)
                return numberOfPeople;

            ShowValidationErrors(errors);
        }
    }

    private static bool CheckFacilityBooking(
        ReservationModel hotelReservation,
        FacilityModel facility,
        FacilityServiceModel service,
        DateTime startTime,
        int numberOfPeople
    )
    {
        ConsoleHelper.ShowHeader("REVIEW YOUR FACILITY BOOKING");
        Console.WriteLine($"  Hotel reservation: {hotelReservation.ReservationCode}");
        Console.WriteLine($"  Facility:          {facility.Name}");
        Console.WriteLine($"  Service:           {service.Name}");
        Console.WriteLine($"  Duration:          {service.DurationMinutes} minutes");
        Console.WriteLine($"  Date:              {startTime:yyyy-MM-dd}");
        Console.WriteLine($"  Start time:        {startTime:HH:mm}");
        Console.WriteLine($"  People:            {numberOfPeople}\n");

        while (true)
        {
            string? choice = ReservationInput.ReadText("Confirm booking? (y/n): ");

            if (choice == null || choice == "n")
                return false;

            if (choice == "y")
                return true;

            Console.WriteLine("  Enter y to confirm, n to abandon, or q to cancel.");
        }
    }

    public static void ShowBooking(FacilityReservationModel booking)
    {
        Console.WriteLine($"    Booking reference: FAC-{booking.FacilityReservationId}");
        Console.WriteLine($"    Facility:          {booking.FacilityName}");
        Console.WriteLine($"    Service:            {booking.ServiceName}");
        Console.WriteLine($"    Date:              {booking.ReservationDate:yyyy-MM-dd}");
        Console.WriteLine($"    Start time:        {booking.StartTime:HH:mm}");
        Console.WriteLine($"    People:            {booking.PartySize}");
        Console.WriteLine($"    Status:            {booking.Status}\n");
    }

    private static void ShowValidationErrors(List<FacilityReservationErrors> errors)
    {
        foreach (var error in errors)
        {
            string message = error switch
            {
                FacilityReservationErrors.AppointmentInPast =>
                    "The appointment cannot start in the past.",
                FacilityReservationErrors.PartySizeBelowOne => "Enter at least one person.",
                FacilityReservationErrors.PartySizeExceedsGuestCount =>
                    "The number of people cannot exceed the guests on your stay.",
                FacilityReservationErrors.InsufficientCapacity =>
                    "There are not enough places available for this group.",
                _ => "Please check your booking details.",
            };

            Console.WriteLine($"  {message}");
        }
    }
}
