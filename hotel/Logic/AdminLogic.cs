using System.Security.Cryptography.X509Certificates;

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

    public List<ReservationDetailModel> ShowAllReservation()
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
        var currentReservation = AdminAccess.GetReservation(reservationId);
        if (currentReservation == null)
        {
            return "Reservation doesn't exist";
        }

        var errors = ReservationValidator.ValidateDate(newCheckInDate, newCheckOutDate, DateTime.Today);
        if (errors.Count > 0)
        {
            return "Invalid dates";
        }
        if (currentReservation.Status == "cancelled")
        {
            return "Reservation is cancelled, cannot edit reservation";
        }

        Dictionary<int, int> roomIdReplacements = new();
        List<int> currentRoomIds = AdminAccess.GetRoomIdsByReservationId(reservationId).ToList();
        foreach (var roomId in currentRoomIds)
        {
            bool foundOtherRoom = false;
            
            bool currentRoomisFree = IsRoomFree(roomId, newCheckInDate, newCheckOutDate, reservationId);
            if (!currentRoomisFree)
            {
                List<int> otherRooms = AdminAccess.GetRoomOfSameType(roomId).ToList();

                foreach (int otherRoomId in otherRooms)
                {
                    if (currentRoomIds.Contains(otherRoomId) || roomIdReplacements.ContainsValue(otherRoomId))
                    {
                        continue;
                    }
                    bool isOtherRoomFree = IsRoomFree(otherRoomId, newCheckInDate, newCheckOutDate, reservationId);
                    if (isOtherRoomFree)
                    {
                        foundOtherRoom = true;
                        roomIdReplacements.Add(roomId, otherRoomId);
                        break;
                    }
                }
                if (!foundOtherRoom)
                {
                    return "No room of this type is available on these dates";
                }
            }
        }

        List<RoomTypeModel> roomTypes = new();
        foreach (var roomId in currentRoomIds)
        {
            int newRoomId = roomId;
            if (roomIdReplacements.ContainsKey(roomId))
            {
                newRoomId = roomIdReplacements[roomId];
            }
            RoomTypeModel? roomType = AdminAccess.GetRoomTypeByRoomId(newRoomId);
            if (roomType == null)
            {
                return "Room not found";
            }
            roomTypes.Add(roomType);
        }

        decimal totalCost = new ReservationLogic().CalculateTotalCost(newCheckInDate, newCheckOutDate, roomTypes);

        currentReservation.CheckInDate = newCheckInDate;
        currentReservation.CheckOutDate = newCheckOutDate;
        currentReservation.TotalCost = totalCost;
        AdminAccess.SaveReservationEdit(currentReservation, roomIdReplacements);

        return "Reservation updated";
    }

    public bool IsRoomFree(int roomid, DateTime newcheckin, DateTime newcheckout, int reservationIdToSkip)
    {
        List<ReservationModel> reservations = AdminAccess.GetReservationsByRoomId(roomid);
        foreach (ReservationModel reservation in reservations)
        {
            if (reservation.Status == "cancelled")
            {
                continue;
            }
            if (reservation.ReservationId == reservationIdToSkip)
            {
                continue;
            }
            if (newcheckin >= reservation.CheckOutDate || newcheckout <= reservation.CheckInDate)
            {
                continue;
            }
            return false;
        }
        return true;
    }
}
