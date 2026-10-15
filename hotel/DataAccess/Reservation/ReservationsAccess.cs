using Dapper;
public static class ReservationsAccess
{
    public static IEnumerable<ReservationModel> GetByUserId(int userId)
    {
        using var connection = DatabaseInitializer.Connect();
        const string sql = @"
            SELECT
                Reservation_Id AS ReservationId,
                User_Id AS UserId,
                CheckInDate,
                CheckOutDate,
                TotalCost,
                Status
            FROM Reservation
            WHERE User_Id = @UserId
            ORDER BY CheckInDate;";

        return connection.Query<ReservationModel>(sql, new { UserId = userId });
    }

    public static bool ReservationCodeExists(string reservationCode)
    {
        using var connection = DatabaseInitializer.Connect();
        const string sql = @"
            SELECT COUNT(*) FROM Reservation
            WHERE ReservationCode = @ReservationCode;";

        return connection.ExecuteScalar<int>(sql, new { ReservationCode = reservationCode }) > 0;
    }

    public static int SaveGuestReservation(UserModel user, ReservationModel reservation,
        List<ReservationRoomModel> rooms)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        const string userSql = @"
            INSERT INTO User (First_Name, Last_Name, Email, Phone, Date_Of_Birth)
            VALUES (@FirstName, @LastName, @Email, @Phone, @DateOfBirth);
            SELECT last_insert_rowid();";

        int userId = connection.QuerySingle<int>(userSql, new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            DateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd")
        }, transaction);

        const string reservationSql = @"
            INSERT INTO Reservation
                (ReservationCode, User_Id, CheckInDate, CheckOutDate, TotalCost, Status)
            VALUES
                (@ReservationCode, @UserId, @CheckInDate, @CheckOutDate, @TotalCost, @Status);
            SELECT last_insert_rowid();";

        int reservationId = connection.QuerySingle<int>(reservationSql, new
        {
            reservation.ReservationCode,
            UserId = userId,
            CheckInDate = reservation.CheckInDate.ToString("yyyy-MM-dd"),
            CheckOutDate = reservation.CheckOutDate.ToString("yyyy-MM-dd"),
            reservation.TotalCost,
            reservation.Status
        }, transaction);

        const string roomSql = @"
            INSERT INTO Reservation_Room (Reservation_Id, Room_Id, Adults, Children)
            VALUES (@ReservationId, @RoomId, @Adults, @Children);";

        foreach (ReservationRoomModel room in rooms)
        {
            connection.Execute(roomSql, new
            {
                ReservationId = reservationId,
                room.RoomId,
                room.Adults,
                room.Children
            }, transaction);
        }

        transaction.Commit();
        return reservationId;
    }
}