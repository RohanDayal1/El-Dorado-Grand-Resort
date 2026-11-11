public static class RoomTypeValidator
{

    public static List<RoomTypeErrors> ValidateRoomType(RoomTypeModel roomtype)
    {
        List<RoomTypeErrors> errors = new();
        if (!ValidateRoomName(roomtype.Name)) {errors.Add(RoomTypeErrors.InvalidName);}
        if (!ValidateDescription(roomtype.Description)) {errors.Add(RoomTypeErrors.InvalidDescription);}
        if (!ValidateCapacity(roomtype.Capacity)) {errors.Add(RoomTypeErrors.InvalidCapacity);}
        if (!ValidatePricePerNight(roomtype.PricePerNight)) {errors.Add(RoomTypeErrors.InvalidPricePerNight);}
        if (!ValidateBedType(roomtype.BedType)) {errors.Add(RoomTypeErrors.InvalidBedType);}
        return errors;
    }

    public static bool ValidateRoomName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        return true;
    }

    public static bool ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return false;
        }
        return true;
    }

    public static bool ValidateCapacity(int capacity)
    {
        if (capacity <= 0 || capacity > 10)
        {
            return false;
        }
        return true;
    }

    public static bool ValidatePricePerNight(decimal pricepernight)
    {
        if (pricepernight <= 0)
        {
            return false;
        }
        return true;
    }

    public static bool ValidateBedType(string bedtype)
    {
        if (string.IsNullOrWhiteSpace(bedtype))
        {
            return false;
        }
        return true;
    }
}