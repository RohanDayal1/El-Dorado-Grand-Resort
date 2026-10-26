public class AdminLogic
{
    public bool IsAdmin(AccountModel? account)
    {
        if (account == null)
        {
            return false;
        }
        return account.Role == "admin";
    }

    public List<UserModel> GetAllGuests()
    {
        return AdminAccess.GetAllGuest();
    }

    public List<ReservationModel> ShowAllReservation()
    {
        return AdminAccess.GetAllReservation();
    }

    public ReservationModel? GetReservation(int id)
    {
        return AdminAccess.GetReservation(id);
    }

    public string CancelReservation(int reservationid)
    {
        var reservation = AdminAccess.GetReservation(reservationid);
        if (reservation == null)
        {
            return "Reservation doesn't exist";
        }
        if (reservation.Status == "cancelled")
        {
            return "Reservation already cancelled";
        }

        AdminAccess.CancelReservation(reservationid);
        return "Reservation cancelled";
    }

    public string EditReservation(int reservationId, DateTime newCheckInDate, DateTime newCheckOutDate)
    {
        var errors = ReservationValidator.ValidateDate(newCheckInDate, newCheckOutDate, DateTime.Today);
        if (errors.Count > 0)
        {
            return "Invalid dates";
        }

        var currentReservation = AdminAccess.GetReservation(reservationId);
        if (currentReservation == null)
        {
            return "Reservation doesn't exist";
        }
        if (currentReservation.Status == "cancelled")
        {
            return "Reservation is cancelled, cannot edit reservation";
        }

        Dictionary<int, int> RoomSwap = new();
        List<int> ids = AdminAccess.GetReservationRoomIds(reservationId).ToList();
        foreach (var roomId in ids)
        {
            bool foundOtherRoom = false;
            
            bool currentRoomisFree = AdminAccess.IsRoomFree(roomId, newCheckInDate, newCheckOutDate, reservationId);
            if (!currentRoomisFree)
            {
                List<int> otherRooms = AdminAccess.GetRoomOfSameType(roomId).ToList();

                foreach (int otherRoomId in otherRooms)
                {
                    if (ids.Contains(otherRoomId) || RoomSwap.ContainsValue(otherRoomId))
                    {
                        continue;
                    }
                    bool isOtherRoomFree = AdminAccess.IsRoomFree(otherRoomId, newCheckInDate, newCheckOutDate, reservationId);
                    if (isOtherRoomFree)
                    {
                        foundOtherRoom = true;
                        RoomSwap.Add(roomId, otherRoomId);
                        break;
                    }
                }
                if (!foundOtherRoom)
                {
                    return "No room of this type is available on these dates";
                }
            }
        }

        currentReservation.CheckInDate = newCheckInDate;
        currentReservation.CheckOutDate = newCheckOutDate;
        AdminAccess.UpdateReservationToDb(currentReservation);

        foreach (KeyValuePair<int, int> pair in RoomSwap)
        {
            AdminAccess.SwapRoom(reservationId, pair.Key, pair.Value);
        }
        return "Reservation updated";
    }
}
