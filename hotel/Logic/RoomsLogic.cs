public class RoomsLogic
{
    public List<RoomTypeModel> GetRoomTypes()
    {
        return RoomsAccess.GetRoomTypes();
    }

    public List<RoomModel> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate)
    {
        return RoomsAccess.GetAvailableRooms(checkInDate, checkOutDate);
    }

    public List<RoomTypeModel> GetAvailableRoomTypes(List<RoomModel> availableRooms,
        List<ReservationRoomModel> selectedRooms, string? preferredBedType)
    {
        var types = GetRoomTypes();
        return types.Where(type => GetAvailableQuantity(type, availableRooms, selectedRooms) > 0)
            .OrderByDescending(type => !string.IsNullOrWhiteSpace(preferredBedType) &&
                string.Equals(type.BedType, preferredBedType, StringComparison.OrdinalIgnoreCase))
            .ThenBy(type => type.PricePerNight).ToList();
    }

    public int GetAvailableQuantity(RoomTypeModel roomType,
        List<RoomModel> availableRooms, List<ReservationRoomModel> selectedRooms)
    {
        int count = 0;
        foreach (var room in availableRooms)
        {
            if (room.Room_Type_Id == roomType.Room_Type_Id &&
                !selectedRooms.Any(selected => selected.RoomId == room.RoomId))
                count++;
        }
        return count;
    }

    public (List<ReservationRoomModel> Rooms, List<RoomQuantityErrors> Errors) SelectRooms(
        RoomTypeModel roomType, int quantity,
        List<RoomModel> availableRooms, List<ReservationRoomModel> selectedRooms)
    {
        int availableQuantity = GetAvailableQuantity(roomType, availableRooms, selectedRooms);
        var result = new List<ReservationRoomModel>();
        var errors = ReservationValidator.ValidateRoomQuantity(quantity, availableQuantity);
        if (errors.Count > 0)
            return (result, errors);
        foreach (var room in availableRooms)
        {
            if (room.Room_Type_Id == roomType.Room_Type_Id &&
                !selectedRooms.Any(selected => selected.RoomId == room.RoomId))
            {
                result.Add(new ReservationRoomModel { RoomId = room.RoomId });
                if (result.Count == quantity)
                    break;
            }
        }
        return (result, errors);
    }
}
