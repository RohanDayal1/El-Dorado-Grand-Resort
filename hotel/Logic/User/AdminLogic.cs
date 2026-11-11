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

    public List<ReservationModel> ShowAllReservation()
    {
        return AdminAccess.GetAllReservation();
    }

    public ReservationModel? GetReservation(string reservationcode)
    {
        return AdminAccess.GetReservation(reservationcode);
    }

    public string CancelReservation(string reservationcode)
    {
        var reservation = AdminAccess.GetReservation(reservationcode);
        if (reservation == null)
        {
            return "Reservation doesn't exist";
        }
        if (reservation.Status == "cancelled")
        {
            return "Reservation already cancelled";
        }
        AdminAccess.CancelReservation(reservationcode);
        return "Reservation cancelled";
    }

    public string EditReservation(string reservationcode, DateTime newCheckInDate, DateTime newCheckOutDate)
    {
        var currentReservation = AdminAccess.GetReservation(reservationcode);
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
        List<int> currentRoomIds = AdminAccess.GetRoomIdsByReservationId(currentReservation.ReservationId).ToList();
        foreach (var roomId in currentRoomIds)
        {
            bool foundOtherRoom = false;
            
            bool currentRoomisFree = IsRoomFree(roomId, newCheckInDate, newCheckOutDate, currentReservation.ReservationId);
            if (!currentRoomisFree)
            {
                List<int> otherRooms = AdminAccess.GetActiveRoomOfSameType(roomId).ToList();

                foreach (int otherRoomId in otherRooms)
                {
                    if (currentRoomIds.Contains(otherRoomId) || roomIdReplacements.ContainsValue(otherRoomId))
                    {
                        continue;
                    }
                    bool isOtherRoomFree = IsRoomFree(otherRoomId, newCheckInDate, newCheckOutDate, currentReservation.ReservationId);
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

    public List<RoomDetailModel> GetAllRooms()
    {
        return AdminAccess.GetAllRoom();
    }

    public List<RoomTypeModel> GetAllRoomTypes()
    {
        return AdminAccess.GetAllRoomType();
    }

    public void AddRoom(RoomModel room)
    {
        AdminAccess.SaveRoom(room);
    }

    public void AddRoomType(RoomTypeModel roomtype)
    {
        AdminAccess.SaveRoomType(roomtype);
    }

    public List<int> GetAllRoomNumbers()
    {
        return AdminAccess.GetAllRoomNumbers();
    }

    public RoomModel? GetRoomByRoomNumber(int roomnumber)
    {
        return AdminAccess.GetRoomByRoomNumber(roomnumber);
    }

    public List<RoomErrors> ValidateRoomDeactivation(int roomnumber)
    {   
        RoomModel? room = AdminAccess.GetRoomByRoomNumber(roomnumber);
        List<ReservationModel> roomReservations = new();
        if (room != null)
        {
            roomReservations = AdminAccess.GetReservationsByRoomId(room.RoomId);
        }
        DateTime today = DateTime.Today;
        List<RoomErrors> errors = RoomValidator.ValidateRoomDeactivation(roomReservations, room, today);
        return errors;
    }

    public void ActivateRoom(int roomnumber)
    {
        AdminAccess.ActivateRoom(roomnumber);
    }

    public void DeactivateRoom(int roomnumber)
    {

        AdminAccess.DeactivateRoom(roomnumber);
    }

    public void EditRoomTypeOfRoom(int roomnumber, int roomtypeid)
    {
        AdminAccess.EditRoomTypeOfRoom(roomnumber, roomtypeid);
    }

    public void EditRoomTypeInformation(RoomTypeModel roomtype)
    {
        AdminAccess.EditRoomTypeInformation(roomtype);
    }

    public List<RoomErrors> ValidateRoom(int roomnumber,List<RoomTypeModel> roomtypes, int roomtypeid, List<int> existingRoomNumbers)
    {
        return RoomValidator.ValidateRoom(roomnumber, roomtypes, roomtypeid, existingRoomNumbers);
    }

    public List<RoomTypeErrors> ValidateRoomType(RoomTypeModel roomtype)
    {
        return RoomTypeValidator.ValidateRoomType(roomtype);
    }

    public bool ValidateRoomTypeId(List<RoomTypeModel> roomtypes, int roomtypeid)
    {
        return RoomValidator.ValidateRoomTypeId(roomtypes, roomtypeid);
    }

}
