public class RoomServiceLogic
{
    public ReservationModel? GetActiveReservation(int userId)
    {
       return RoomServiceAccess.GetActiveReservation(userId);
    }

    public List<int> GetRoomNumbers(int reservationId)
    {
        return RoomServiceAccess.GetRoomNumbers(reservationId);
    }

    public List<RoomServiceOrderModel> GetOrders(int reservationId)
    {
        return RoomServiceAccess.GetOrders(reservationId);
    }

    public List<RoomServiceModel> GetFoodAndDrinks()
    {
        return RoomServiceAccess.GetFoodAndDrinks();
    }
    
    public List<RoomServiceModel> GetHousekeeping()
    {
        return new List<RoomServiceModel>
        {
            new RoomServiceModel("Room cleaning", "Request room cleaning", 0, 0)
        };
    }

    public List<RoomServiceModel> GetExtraTowels()
    {
        return new List<RoomServiceModel>
        {
            new RoomServiceModel("Extra towels", "Request fresh towels for your room", 0, 0)
        };
    }

    public bool ValidateOrder(List<RoomServiceModel> items)
    {
        foreach (RoomServiceModel item in items)
        {
            if (item.Quantity < 0)
            {
                return false;
            }
        }

        return items.Count > 0;
    }

    public decimal CalculateTotal(List<RoomServiceModel> items)
    {
        decimal total = 0;

        foreach (RoomServiceModel item in items)
        {
            total += (decimal)item.Price * item.Quantity;
        }

        return total;
    }


    public int PlaceOrder(ReservationModel reservation, int roomNumber, List<RoomServiceModel> items)
    {
        if (reservation.Status != "confirmed" || reservation.CheckInDate.Date > DateTime.Today || reservation.CheckOutDate.Date <= DateTime.Today)
        {
            return -1;
        }

        if (!ValidateOrder(items))
        {
            return -1;
        }

        RoomServiceOrderModel order = new RoomServiceOrderModel
        {
            ReservationId = reservation.ReservationId,
            RoomNumber = roomNumber,
            Date = DateTime.Now,
            Status = "Pending",
            Total = CalculateTotal(items)
        };

        foreach (RoomServiceModel item in items)
        {
            if (item.Quantity > 0)
            {
                order.Items.Add(new RoomServiceOrderItemModel
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = (decimal)item.Price
                });
            }
        }

        return RoomServiceAccess.SaveOrder(order);
    }
}