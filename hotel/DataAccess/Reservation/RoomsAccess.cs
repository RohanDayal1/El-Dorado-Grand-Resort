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

    public static List<RoomModel> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate, int? reservationId = null)
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
                  AND (@ReservationId IS NULL OR r.Reservation_Id != @ReservationId)
            )
            AND Status = @status
            ORDER BY RoomNumber;",
                new
                {
                    CheckIn = checkInDate.ToString("yyyy-MM-dd"),
                    CheckOut = checkOutDate.ToString("yyyy-MM-dd"),
                    ReservationId = reservationId,
                    status = "active"
                }
            )
            .ToList();
    }

    public static List<RoomModel> GetByReservationId(int reservationId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.Query<RoomModel>(
            @"
            SELECT rm.Room_Id AS RoomId, rm.Room_Type_Id, rm.RoomNumber
            FROM Reservation_Room rr
            JOIN Room rm ON rm.Room_Id = rr.Room_Id
            WHERE rr.Reservation_Id = @ReservationId
            ORDER BY rm.RoomNumber;",
            new { ReservationId = reservationId }
        ).ToList();
    }
}
