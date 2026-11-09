using Dapper;
using Microsoft.Data.Sqlite;

public static class ReservationsAccess
{
    public static IEnumerable<ReservationModel> GetByUserId(int userId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.Query<ReservationModel>(
            @"
            SELECT Reservation_Id AS ReservationId, ReservationCode, User_Id AS UserId,
                   CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status
            FROM Reservation
            WHERE User_Id = @UserId
            ORDER BY CheckInDate;",
            new { UserId = userId }
        );
    }

    public static bool ReservationCodeExists(string reservationCode)
    {
        using var connection = DatabaseInitializer.Connect();
        return connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Reservation WHERE ReservationCode = @ReservationCode;",
                new { ReservationCode = reservationCode }
            ) > 0;
    }

    public static int SaveGuestReservation(
        UserModel user,
        ReservationModel reservation,
        List<ReservationRoomModel> rooms
    )
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        int userId = connection.QuerySingle<int>(
            @"
            INSERT INTO User (First_Name, Last_Name, Email, Phone, Date_Of_Birth)
            VALUES (@FirstName, @LastName, @Email, @Phone, @DateOfBirth);
            SELECT last_insert_rowid();",
            new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                user.Phone,
                DateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
            },
            transaction
        );
        int reservationId = InsertReservation(connection, transaction, userId, reservation, rooms);
        transaction.Commit();
        reservation.UserId = userId;
        return reservationId;
    }

    public static int SaveReservationForUser(
        int userId,
        ReservationModel reservation,
        List<ReservationRoomModel> rooms
    )
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        int reservationId = InsertReservation(connection, transaction, userId, reservation, rooms);
        transaction.Commit();
        reservation.UserId = userId;
        return reservationId;
    }

    private static int InsertReservation(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int userId,
        ReservationModel reservation,
        List<ReservationRoomModel> rooms
    )
    {
        int reservationId = connection.QuerySingle<int>(
            @"
            INSERT INTO Reservation
                (ReservationCode, User_Id, CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status)
            VALUES
                (@ReservationCode, @UserId, @CheckInDate, @CheckOutDate, @Adults, @Children, @TotalCost, @Status);
            SELECT last_insert_rowid();",
            new
            {
                reservation.ReservationCode,
                UserId = userId,
                CheckInDate = reservation.CheckInDate.ToString("yyyy-MM-dd"),
                CheckOutDate = reservation.CheckOutDate.ToString("yyyy-MM-dd"),
                reservation.Adults,
                reservation.Children,
                reservation.TotalCost,
                reservation.Status,
            },
            transaction
        );
        foreach (var room in rooms)
        {
            connection.Execute(
                @"
            INSERT INTO Reservation_Room (Reservation_Id, Room_Id)
            VALUES (@ReservationId, @RoomId);",
                new { ReservationId = reservationId, room.RoomId },
                transaction
            );
        }
        return reservationId;
    }

    public static ReservationModel? GetByReservationCode(string reservationCode)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.QueryFirstOrDefault<ReservationModel>(@"
        SELECT Reservation_Id AS ReservationId, ReservationCode, User_Id AS UserId, CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status
        FROM Reservation
        WHERE ReservationCode = @ReservationCode;", new {ReservationCode = reservationCode});
    }

    public static List<ReservationRoomModel> GetRooms(int reservationId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.Query<ReservationRoomModel>(@"
        SELECT Reservation_Id AS ReservationId, Room_Id AS RoomId
        FROM Reservation_Room
        WHERE Reservation_Id = @ReservationId;", new { ReservationId = reservationId } ).ToList();
    }

    public static void UpdateReservation(ReservationModel reservation, List<ReservationRoomModel> rooms)
    {
        using var connection = DatabaseInitializer.Connect();

        string sql = "UPDATE Reservation SET CheckOutDate = @CheckOutDate, TotalCost = @TotalCost WHERE Reservation_Id = @ReservationId";
        connection.Execute(sql, reservation);

        sql = "DELETE FROM Reservation_Room WHERE Reservation_Id = @ReservationId";
        connection.Execute(sql, reservation);

        foreach (ReservationRoomModel room in rooms)
        {
            sql = "INSERT INTO Reservation_Room (Reservation_Id, Room_Id) VALUES (@ReservationId, @RoomId)";
            connection.Execute(sql, room);
        }
    }
}
