using Dapper;
using Microsoft.Data.Sqlite;

public static class ReservationsAccess
{
    public static IEnumerable<ReservationModel> GetByUserId(int userId)
    {
        using var connection = DatabaseInitializer.Connect();
        const string sql = @"
            SELECT Reservation_Id AS ReservationId, ReservationCode, User_Id AS UserId,
                   CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status
            FROM Reservation
            WHERE User_Id = @UserId
            ORDER BY CheckInDate;";
        return connection.Query<ReservationModel>(sql, new { UserId = userId });
    }

    public static bool ReservationCodeExists(string reservationCode)
    {
        using var connection = DatabaseInitializer.Connect();
        const string sql = "SELECT COUNT(*) FROM Reservation WHERE ReservationCode = @ReservationCode;";
        return connection.ExecuteScalar<int>(sql, new { ReservationCode = reservationCode }) > 0;
    }

    public static int SaveGuestReservation(UserModel user, ReservationModel reservation,
        List<ReservationRoomModel> rooms)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        const string sql = @"
            INSERT INTO User (First_Name, Last_Name, Email, Phone, Date_Of_Birth)
            VALUES (@FirstName, @LastName, @Email, @Phone, @DateOfBirth);
            SELECT last_insert_rowid();";
        int userId = connection.QuerySingle<int>(sql, new
        {
            user.FirstName, user.LastName, user.Email, user.Phone,
            DateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd")
        }, transaction);
        int reservationId = InsertReservation(connection, transaction, userId, reservation, rooms);
        transaction.Commit();
        reservation.UserId = userId;
        return reservationId;
    }

    public static int SaveReservationForUser(int userId, ReservationModel reservation,
        List<ReservationRoomModel> rooms)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        int reservationId = InsertReservation(connection, transaction, userId, reservation, rooms);
        transaction.Commit();
        reservation.UserId = userId;
        return reservationId;
    }

    private static int InsertReservation(SqliteConnection connection, SqliteTransaction transaction,
        int userId, ReservationModel reservation, List<ReservationRoomModel> rooms)
    {
        const string sql = @"
            INSERT INTO Reservation
                (ReservationCode, User_Id, CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status)
            VALUES
                (@ReservationCode, @UserId, @CheckInDate, @CheckOutDate, @Adults, @Children, @TotalCost, @Status);
            SELECT last_insert_rowid();";
        int reservationId = connection.QuerySingle<int>(sql, new
        {
            reservation.ReservationCode,
            UserId = userId,
            CheckInDate = reservation.CheckInDate.ToString("yyyy-MM-dd"),
            CheckOutDate = reservation.CheckOutDate.ToString("yyyy-MM-dd"),
            reservation.Adults, reservation.Children, reservation.TotalCost, reservation.Status
        }, transaction);
        const string roomSql = @"
            INSERT INTO Reservation_Room (Reservation_Id, Room_Id)
            VALUES (@ReservationId, @RoomId);";
        foreach (var room in rooms)
        {
            connection.Execute(roomSql, new { ReservationId = reservationId, room.RoomId }, transaction);
        }
        return reservationId;
    }
}
