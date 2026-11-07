public static class RoomTypeValidator
{

    public static List<RoomTypeErrors> ValidateRoomType(string roomname, string description, int capacity, decimal pricepernight, string bedtype)
    {
        var errors = new List<RoomTypeErrors>();
        if (!ValidateRoomName(roomname)) {errors.Add(RoomTypeErrors.InvalidName);}
        if (!ValidateDescription(description)) {errors.Add(RoomTypeErrors.InvalidDescription);}
        if (!ValidateCapacity(capacity)) {errors.Add(RoomTypeErrors.InvalidCapacity);}
        if (!ValidatePricePerNight(pricepernight)) {errors.Add(RoomTypeErrors.InvalidPricePerNight);}
        if (!ValidateBedType(bedtype)) {errors.Add(RoomTypeErrors.InvalidBedType);}
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