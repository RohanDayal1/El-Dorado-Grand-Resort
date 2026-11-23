
public static class RoomService
{
    private static readonly RoomServiceLogic _logic = new RoomServiceLogic();

    public static void Start(AccountModel account)
    {
        ReservationModel? reservation = _logic.GetActiveReservation(account.UserId);

        if (reservation == null)
        {
            ConsoleHelper.ShowHeader("ROOM SERVICE");
            Console.WriteLine("  Room service is only available during an active stay.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        List<int> roomNumbers = _logic.GetRoomNumbers(reservation.ReservationId);

        if (roomNumbers.Count == 0)
        {
            Console.WriteLine("  Your room number could not be found.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        int roomNumber = roomNumbers[0];

        if (roomNumbers.Count > 1)
        {
            List<string> options = roomNumbers.Select(number => $"Room {number}").ToList();

            int choice = ConsoleHelper.SelectionMenu(options, "ROOM SERVICE", "Which room would you like the order delivered to?");

            if (choice < 0)
                return;

            roomNumber = roomNumbers[choice];
        }

        ShowSelection(reservation, roomNumber);
    }

    public static void ShowSelection(ReservationModel reservation, int roomNumber)
    {
        while (true)
        {
            List<string> options = new List<string>
            {
                "Food & Drinks",
                "Housekeeping",
                "Extra Towels",
                "My Requests"
            };

            int choice = ConsoleHelper.SelectionMenu(options, "ROOM SERVICE", "Choose a room service:", null,
                $"Your room number: {roomNumber}\n\nFor additional questions, please contact our Service Desk.\n");

            switch (choice)
            {
                case 0:
                    CreateOrder(reservation, roomNumber, _logic.GetFoodAndDrinks());
                    break;

                case 1:
                    CreateOrder(reservation, roomNumber, _logic.GetHousekeeping());
                    break;

                case 2:
                    CreateOrder(reservation, roomNumber, _logic.GetExtraTowels());
                    break;

                case 3:
                    ShowOrders(reservation.ReservationId);
                    break;

                default:
                    return;
            }
        }
    }

    private static void CreateOrder(ReservationModel reservation, int roomNumber, List<RoomServiceModel> availableItems)
    {
        List<string> details = availableItems.Select(item => $"{item.Description} | €{item.Price:F2}").ToList();

        while (true)
        {
            List<string> options = availableItems.Select(item => item.Quantity > 0 ? $"{item.Name} (Added: {item.Quantity})" : item.Name).ToList();

            int choice = ConsoleHelper.SelectionMenu(options, "ROOM SERVICE", "Select the items you want:", details, "Press C to finish the order selection.\n");

            if (choice == -1 || choice == -2)
                break;

            availableItems[choice].Quantity++;
        }

        List<RoomServiceModel> orderItems = availableItems.Where(item => item.Quantity > 0).ToList();

        if (orderItems.Count == 0)
            return;

        decimal total = _logic.CalculateTotal(orderItems);

        string summary = "";
        foreach (RoomServiceModel item in orderItems)
        {
            summary += $"{item.Name} x {item.Quantity} | €{(item.Price * item.Quantity):F2}\n";
        }

        List<string> confirmationOptions = new List<string> { "Yes", "No" };

        int roomConfirmation = ConsoleHelper.SelectionMenu(confirmationOptions, "CONFIRM ROOM", "Please confirm your room number.", true, $"Your room number: {roomNumber}");

        if (roomConfirmation != 0)
            return;

        int confirmation = ConsoleHelper.SelectionMenu(confirmationOptions, "ORDER SUMMARY", "Would you like to confirm your order?", true, $"{summary}\nTotal: €{total:F2}");

        if (confirmation != 0)
        {
            Console.WriteLine("  Your order was cancelled.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        int orderId = _logic.PlaceOrder(reservation, roomNumber, orderItems);

        if (orderId != -1)
        {
            Console.WriteLine($"\n  Your request has been successfully submitted!");
            Console.WriteLine($"  Request number: {orderId}");
            Console.WriteLine($"  Room number: {roomNumber}");
            Console.WriteLine("  Status: Pending");
        }
        else
        {
            Console.WriteLine("  Your request could not be submitted. Please try again.");
        }

        ConsoleHelper.WaitForEnter();
    }


    private static void ShowOrders(int reservationId)
    {
        ConsoleHelper.ShowHeader("MY ROOM SERVICE REQUESTS");

        List<RoomServiceOrderModel> orders = _logic.GetOrders(reservationId);

        if (orders.Count == 0)
        {
            Console.WriteLine("  You have no room service requests yet.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        foreach (RoomServiceOrderModel order in orders)
        {
            Console.WriteLine($"  Request number: {order.Id}");
            Console.WriteLine($"  Date: {order.Date}");
            Console.WriteLine($"  Status: {order.Status}");
            Console.WriteLine($"  Total: €{order.Total:F2}\n");

            foreach (RoomServiceOrderItemModel item in order.Items)
            {
                Console.WriteLine($"    {item.Name} x {item.Quantity}");
            }

            Console.WriteLine();
        }

        ConsoleHelper.WaitForEnter();
    }
}
