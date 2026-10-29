using Microsoft.Data.Sqlite;
using Dapper;
using System.Data;

public static class AdminAccess
{
    public static List<UserModel> GetAllGuest()
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        const string sql = @"SELECT User_Id AS UserId,
                        First_Name AS FirstName,
                        Last_Name AS LastName,
                        Email,
                        Phone,
                        Date_Of_Birth AS DateOfBirth
                        FROM User";

        List<UserModel> AllGuests = connection.Query<UserModel>(sql).ToList();
        return AllGuests;
    }

    public static List<ReservationModel> GetAllReservation()
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        const string sql = @"SELECT Reservation_Id AS ReservationId,
                            ReservationCode,
                            Reservation.User_Id as UserId,
                            User.First_Name as FirstName,
                            User.Last_Name as LastName,
                            CheckInDate,
                            CheckOutDate,
                            Adults,
                            Children,
                            TotalCost,
                            Status
                            FROM Reservation
                            INNER JOIN User ON Reservation.User_Id = User.User_Id";
    
        List<ReservationModel> AllReservation = connection.Query<ReservationModel>(sql).ToList();
        return AllReservation;
    }

    public static ReservationModel? GetReservation(int id)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        const string sql = @"SELECT Reservation_Id AS ReservationId,
                            ReservationCode,
                            User_Id AS UserId,
                            CheckInDate,
                            CheckOutDate,
                            Adults,
                            Children,
                            TotalCost,
                            Status
                            FROM Reservation
                            WHERE Reservation_Id = @reservationid";
        ReservationModel? reservation = connection.QueryFirstOrDefault<ReservationModel>(sql, new {reservationid = id});
        return reservation;
    }

    public static void CancelReservation(int id)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        const string sql = @"UPDATE Reservation SET Status = 'cancelled' WHERE Reservation_Id = @ReservationId";
        connection.Execute(sql, new {ReservationId = id});
    }

    public static void SaveReservationEdit(ReservationModel reservation, Dictionary<int, int> roomIdReplacements)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        const string sql = @"UPDATE Reservation 
                    SET CheckInDate = @CheckInDate, 
                    CheckOutDate = @CheckOutDate,
                    TotalCost = @TotalCost
                    WHERE Reservation_Id = @ReservationId";
        connection.Execute(sql, 
        new
        {
            CheckInDate = reservation.CheckInDate.ToString("yyyy-MM-dd"),
            CheckOutDate = reservation.CheckOutDate.ToString("yyyy-MM-dd"),
            reservation.TotalCost,
            reservation.ReservationId
        }, transaction);

        foreach (var (oldroomid, newroomid) in roomIdReplacements)
        {
            SwapRoom(connection, transaction, reservation.ReservationId, oldroomid, newroomid);
        }
        transaction.Commit();
    }

    public static void SwapRoom(
        SqliteConnection connection, 
        SqliteTransaction transaction,
        int reservationid, 
        int oldroomid, 
        int newroomid)
    {
        const string sql = @"UPDATE Reservation_Room
                            SET Room_Id = @newRoomId
                            WHERE Reservation_Id = @ReservationId 
                            AND Room_Id = @oldRoomId";
        connection.Execute(sql, 
        new 
        {
            newRoomId = newroomid,
            ReservationId = reservationid,
            oldRoomId = oldroomid
        }, transaction);
    }

    public static List<int> GetRoomIdsByReservationId(int id)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        List<int> room = connection.Query<int>(@"SELECT Room_Id 
                                                FROM Reservation_Room 
                                                WHERE Reservation_Id = @reservationId", 
                                                new {reservationId = id}).ToList();
        return room;
    }

    public static List<ReservationModel> GetReservationsByRoomId(int roomid)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        const string sql = @"SELECT Reservation.Reservation_Id AS ReservationId, 
                            Reservation.CheckInDate, 
                            Reservation.CheckOutDate, 
                            TotalCost, 
                            Status
                            FROM Reservation 
                            INNER JOIN Reservation_Room ON Reservation.Reservation_Id = Reservation_Room.Reservation_Id
                            WHERE Reservation_Room.Room_Id = @roomId";

        List<ReservationModel> reservations = connection.Query<ReservationModel>(sql, new {roomId = roomid}).ToList();
        return reservations;
    }

    public static List<int> GetRoomOfSameType(int roomid)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        const string sql = @"SELECT Room_Id AS RoomId 
                            FROM Room 
                            WHERE Room_Type_Id = (SELECT Room_Type_id FROM Room WHERE Room_Id = @roomId)";

        List<int> roomIdWithSameType = connection.Query<int>(sql, new {roomId = roomid}).ToList();
        return roomIdWithSameType;
    }

    public static RoomTypeModel? GetRoomTypeByRoomId(int roomid)
    {
        using var connection = DatabaseInitializer.Connect();
        const string sql = @"SELECT rt.Room_Type_Id, 
                            rt.Name, 
                            rt.Description, 
                            rt.Capacity, 
                            rt.PricePerNight,
                            rt.BedType
                            FROM Room
                            INNER JOIN Room_Type rt on Room.Room_Type_Id = rt.Room_Type_Id
                            WHERE Room_Id = @RoomId";

        RoomTypeModel? roomType = connection.QueryFirstOrDefault<RoomTypeModel>(sql, new {RoomId = roomid});
        return roomType;
    }
}