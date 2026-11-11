public static class ReservationOverview
{
    private const string Border = "============================================================";

    public static void LookupReservation()
    {
        ConsoleHelper.ClearScreen();
        Console.WriteLine(Border);
        Console.WriteLine("  EL DORADO GRAND RESORT  |  RESERVATION OVERVIEW");
        Console.WriteLine(Border);
        Console.WriteLine();
        Console.WriteLine("  Enter the details used when making the reservation.");
        Console.WriteLine();
        Console.Write("  Email address: ");
        string email = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.WriteLine();
        Console.Write("  Reservation code: ");
        string reservationCode = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(reservationCode))
        {
            ShowLookupMessage("Enter both your email address and reservation code.");
            ConsoleHelper.WaitForEnter();
            ConsoleHelper.ClearScreen();
            return;
        }

        ReservationModel? reservation = new ReservationLogic()
            .GetOverviewByEmailAndCode(email, reservationCode);
        if (reservation == null)
        {
            ShowLookupMessage("No reservation matched that email address and code.");
            ConsoleHelper.WaitForEnter();
            ConsoleHelper.ClearScreen();
            return;
        }

        ShowReservationOverview(reservation);
    }

    private static void ShowLookupMessage(string message)
    {
        ConsoleHelper.ClearScreen();
        Console.WriteLine(Border);
        Console.WriteLine("  EL DORADO GRAND RESORT  |  RESERVATION OVERVIEW");
        Console.WriteLine(Border);
        Console.WriteLine();
        Console.WriteLine($"  {message}");
    }

    private static void ShowReservationOverview(ReservationModel reservation)
    {
        var roomsLogic = new RoomsLogic();
        List<RoomTypeModel> roomTypes = roomsLogic.GetRoomTypes();
        List<RoomModel> rooms = roomsLogic.GetByReservationId(reservation.ReservationId);
        List<FacilityReservationModel> facilities = new FacilityReservationLogic()
            .GetByReservationId(reservation.ReservationId);

        ConsoleHelper.ClearScreen();
        Console.WriteLine(Border);
        Console.WriteLine("  EL DORADO GRAND RESORT  |  RESERVATION OVERVIEW");
        Console.WriteLine(Border);
        Console.WriteLine();
        Console.WriteLine("  RESERVATION");
        Console.WriteLine("  ------------------------------------------------------------");
        Console.WriteLine($"  Guest:            {reservation.FirstName} {reservation.LastName}");
        Console.WriteLine($"  Reservation code: {reservation.ReservationCode}");
        Console.WriteLine($"  Status:           {reservation.Status}");
        Console.WriteLine();
        Console.WriteLine("  STAY AND GUESTS");
        Console.WriteLine("  ------------------------------------------------------------");
        Console.WriteLine($"  Check-in:         {reservation.CheckInDate:yyyy-MM-dd}");
        Console.WriteLine($"  Check-out:        {reservation.CheckOutDate:yyyy-MM-dd}");
        Console.WriteLine($"  Length of stay:   {(reservation.CheckOutDate - reservation.CheckInDate).Days} nights");
        Console.WriteLine($"  Adults:           {reservation.Adults}");
        Console.WriteLine($"  Children:         {reservation.Children}");
        Console.WriteLine();
        Console.WriteLine("  SELECTED ROOMS");
        Console.WriteLine("  ------------------------------------------------------------");
        if (rooms.Count == 0)
        {
            Console.WriteLine("  No rooms are listed for this reservation.");
        }
        else
        {
            foreach (RoomModel room in rooms)
            {
                RoomTypeModel? roomType = roomTypes.FirstOrDefault(
                    type => type.Room_Type_Id == room.Room_Type_Id
                );
                Console.WriteLine($"  {roomType?.Name ?? "Room"}  |  Room {room.RoomNumber}");
                if (!string.IsNullOrWhiteSpace(roomType?.BedType))
                    Console.WriteLine($"  Bed type: {roomType.BedType}");
                if (!string.IsNullOrWhiteSpace(roomType?.Description))
                    Console.WriteLine($"  {roomType.Description}");
                Console.WriteLine();
            }
        }

        Console.WriteLine("  BOOKED FACILITIES");
        Console.WriteLine("  ------------------------------------------------------------");
        if (facilities.Count == 0)
        {
            Console.WriteLine("  No facilities are linked to this reservation.");
        }
        else
        {
            foreach (FacilityReservationModel facility in facilities)
            {
                Console.WriteLine($"  {facility.FacilityName}  |  {facility.ServiceName}");
                Console.WriteLine(
                    $"  Date: {facility.ReservationDate:yyyy-MM-dd}  |  Time: {facility.StartTime:HH:mm}-{facility.EndTime:HH:mm}"
                );
                Console.WriteLine($"  Party size: {facility.PartySize}  |  Status: {facility.Status}");
                Console.WriteLine();
            }
        }

        Console.WriteLine(Border);
        Console.WriteLine($"  TOTAL RESERVATION COST: {reservation.TotalCost:C}");
        Console.WriteLine();
        Console.Write("  Press Enter to return to the home menu...");
        Console.ReadLine();
        ConsoleHelper.ClearScreen();
    }
}
