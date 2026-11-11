using System.Globalization;

public static class RoomValidator
{
    public static List<RoomErrors> ValidateRoom
        (
        int roomNumber,
        List<RoomTypeModel> roomtypes, 
        int roomTypeId, 
        List<int> existingRoomNumbers
        )
    {
        List<RoomErrors> errors = new();
        if (!ValidateFloor(roomNumber)) {errors.Add(RoomErrors.InvalidFloor);}
        if (!ValidateRoomNumber(roomNumber)) {errors.Add(RoomErrors.InvalidRoomNumber);}
        if (!ValidateRoomTypeId(roomtypes, roomTypeId)) {errors.Add(RoomErrors.InvalidRoomType);}
        if (!ValidateRoomNumberAvailable(existingRoomNumbers, roomNumber)) {errors.Add(RoomErrors.RoomNumberTaken);}
        return errors;
    }
    public static bool ValidateFloor(int roomnumber)
    {
        int floor = roomnumber/ 100;
        if (floor < 1 || floor > 4)
        {
            return false;
        }
        return true;
    }

    public static bool ValidateRoomNumber(int roomnumber)
    {
        int number = roomnumber % 100;
        if (number < 1 || number > 20)
        {
            return false;
        }
        return true;
    }

    public static bool ValidateRoomNumberAvailable(List<int> existingRoomNumbers, int roomNumber)
    {
        if (!existingRoomNumbers.Contains(roomNumber))
        {
            return true;
        }
        return false;
    }

    public static bool ValidateRoomTypeId(List<RoomTypeModel> roomtypes, int roomtypeId)
    {
        List<int> allRoomTypeIds = new();
        foreach (var room in roomtypes)
        {
            allRoomTypeIds.Add(room.Room_Type_Id);
        }
        if (!allRoomTypeIds.Contains(roomtypeId))
        {
            return false;
        }
        return true;
    }

    public static List<RoomErrors> ValidateRoomDeactivation(List<ReservationModel> reservations, RoomModel? room, DateTime today)
    {
        List<RoomErrors> errors = new();
        if (room == null)
        {
            errors.Add(RoomErrors.RoomNotFound);
            return errors;
        }
        foreach (var reservation in reservations)
        {
            if (reservation.Status == "confirmed" && reservation.CheckOutDate >= today)
            {
                errors.Add(RoomErrors.RoomHasReservations);
                break;
            }
        }
        return errors;
    }
}