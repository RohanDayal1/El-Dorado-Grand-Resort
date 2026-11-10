using Dapper;
using Microsoft.Data.Sqlite;

public static class ReservationsAccess
{
    public static ReservationModel? GetById(int reservationId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.QueryFirstOrDefault<ReservationModel>(
            @"
            SELECT Reservation_Id AS ReservationId, ReservationCode, User_Id AS UserId,
                   CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status
            FROM Reservation
            WHERE Reservation_Id = @ReservationId;",
            new { ReservationId = reservationId }
        );
    }

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

    public static ReservationOverviewModel? GetOverviewByEmailAndCode(
        string email,
        string reservationCode
    )
    {
        using var connection = DatabaseInitializer.Connect();
        ReservationOverviewModel? overview = connection.QuerySingleOrDefault<ReservationOverviewModel>(
            @"
            SELECT r.Reservation_Id AS ReservationId,
                   r.ReservationCode,
                   r.User_Id AS UserId,
                   u.First_Name || ' ' || u.Last_Name AS GuestName,
                   u.Email,
                   r.CheckInDate,
                   r.CheckOutDate,
                   r.Adults,
                   r.Children,
                   r.TotalCost,
                   r.Status
            FROM Reservation r
            JOIN User u ON u.User_Id = r.User_Id
            WHERE lower(u.Email) = lower(@Email)
              AND upper(r.ReservationCode) = upper(@ReservationCode);",
            new { Email = email.Trim(), ReservationCode = reservationCode.Trim() }
        );

        if (overview == null)
            return null;

        overview.Rooms = connection.Query<ReservationRoomDetailModel>(
            @"
            SELECT rm.RoomNumber,
                   rt.Name AS RoomTypeName,
                   rt.Description,
                   rt.BedType
            FROM Reservation_Room rr
            JOIN Room rm ON rm.Room_Id = rr.Room_Id
            JOIN Room_Type rt ON rt.Room_Type_Id = rm.Room_Type_Id
            WHERE rr.Reservation_Id = @ReservationId
            ORDER BY rm.RoomNumber;",
            new { overview.ReservationId }
        ).ToList();

        overview.Facilities = connection.Query<FacilityReservationDetailModel>(
            @"
            SELECT f.Name AS FacilityName,
                   fs.Name AS ServiceName,
                   f.Description AS FacilityDescription,
                   fs.Description AS ServiceDescription,
                   fr.ReservationDate,
                   fr.StartTime,
                   fr.EndTime,
                   fr.PartySize,
                   fr.Status
            FROM Facility_Reservation fr
            JOIN Facility f ON f.Facility_Id = fr.Facility_Id
            JOIN Facility_Service fs ON fs.Facility_Service_Id = fr.Facility_Service_Id
            WHERE fr.Reservation_Id = @ReservationId
            ORDER BY fr.ReservationDate, fr.StartTime;",
            new { overview.ReservationId }
        ).ToList();

        return overview;
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
}
