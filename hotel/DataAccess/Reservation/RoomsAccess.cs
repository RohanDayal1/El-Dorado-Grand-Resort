using Dapper;

public static class RoomsAccess
{
    public static List<RoomTypeModel> GetRoomTypes()
    {
        using var connection = DatabaseInitializer.Connect();

        return connection
            .Query<RoomTypeModel>(
                @"
            SELECT Room_Type_Id, Name, Description,
                   Capacity, PricePerNight, BedType
            FROM Room_Type;"
            )
            .ToList();
    }

    public static List<RoomModel> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection
            .Query<RoomModel>(
                @"
            SELECT Room_Id AS RoomId, Room_Type_Id, RoomNumber
            FROM Room
            WHERE Room_Id NOT IN (
                SELECT rr.Room_Id
                FROM Reservation_Room rr
                JOIN Reservation r ON r.Reservation_Id = rr.Reservation_Id
                WHERE r.Status = 'confirmed'
                  AND r.CheckInDate < @CheckOut
                  AND r.CheckOutDate > @CheckIn
            )
            ORDER BY RoomNumber;",
                new
                {
                    CheckIn = checkInDate.ToString("yyyy-MM-dd"),
                    CheckOut = checkOutDate.ToString("yyyy-MM-dd"),
                }
            )
            .ToList();
    }
}
