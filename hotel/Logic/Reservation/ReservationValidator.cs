public static class ReservationValidator
{
    public static List<RoomSelectionErrors> ValidateRoomSelection(
        int adults,
        int children,
        int roomCount,
        int totalCapacity
    )
    {
        var errors = new List<RoomSelectionErrors>();

        if (roomCount < 1)
            errors.Add(RoomSelectionErrors.NoRoomsSelected);

        if (roomCount > adults)
            errors.Add(RoomSelectionErrors.MoreRoomsThanAdults);

        if (totalCapacity > 0 && adults + children > totalCapacity)
            errors.Add(RoomSelectionErrors.InsufficientCapacity);

        return errors;
    }

    public static List<RoomQuantityErrors> ValidateRoomQuantity(int quantity, int availableQuantity)
    {
        var errors = new List<RoomQuantityErrors>();

        if (quantity < 1)
            errors.Add(RoomQuantityErrors.QuantityBelowOne);

        if (quantity > availableQuantity)
            errors.Add(RoomQuantityErrors.QuantityExceedsAvailability);

        return errors;
    }

    public static List<RoomSelectionErrors> ValidateSelectedRoomData(
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes
    )
    {
        var errors = new List<RoomSelectionErrors>();

        if (selectedRooms.GroupBy(room => room.RoomId).Any(group => group.Count() > 1))
            errors.Add(RoomSelectionErrors.DuplicateRoom);

        if (
            selectedRooms.Any(selected =>
                !availableRooms.Any(room => room.RoomId == selected.RoomId)
            )
        )
            errors.Add(RoomSelectionErrors.RoomUnavailable);

        if (
            selectedRooms.Any(selected =>
            {
                var room = availableRooms.FirstOrDefault(room => room.RoomId == selected.RoomId);
                return room != null
                    && !roomTypes.Any(type => type.Room_Type_Id == room.Room_Type_Id);
            })
        )
            errors.Add(RoomSelectionErrors.RoomTypeNotFound);

        return errors;
    }

    public static List<ReservationDateErrors> ValidateDate(
        DateTime checkInDate,
        DateTime checkOutDate,
        DateTime today
    )
    {
        var errors = new List<ReservationDateErrors>();

        if (checkInDate.Date < today.Date)
            errors.Add(ReservationDateErrors.CheckInDateInPast);

        if (checkOutDate.Date <= checkInDate.Date)
            errors.Add(ReservationDateErrors.CheckOutDateNotAfterCheckIn);

        return errors;
    }
}
