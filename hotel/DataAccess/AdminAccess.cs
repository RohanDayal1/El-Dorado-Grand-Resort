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
                            User_Id AS UserId,
                            CheckInDate,
                            CheckOutDate,
                            TotalCost,
                            Status
                            FROM Reservation";
    
        List<ReservationModel> AllReservation = connection.Query<ReservationModel>(sql).ToList();
        return AllReservation;
    }

    public static void CancelReservation(int id)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        const string sql = @"UPDATE Reservation SET Status = 'cancelled' WHERE Reservation_Id = @ReservationId";
        connection.Execute(sql, new {ReservationId = id});
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
                            TotalCost,
                            Status
                            FROM Reservation
                            WHERE Reservation_Id = @reservationid";
        ReservationModel? reservation = connection.QueryFirstOrDefault<ReservationModel>(sql, new {reservationid = id});
        return reservation;
    }

    public static void UpdateReservationToDb(ReservationModel reservation)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        const string sql = @"UPDATE Reservation 
                    SET CheckInDate = @CheckInDate, 
                    CheckOutDate = @CheckOutDate
                    WHERE Reservation_Id = @ReservationId";
        connection.Execute(sql, 
        new
        {
            CheckInDate = reservation.CheckInDate.ToString("yyyy-MM-dd"),
            CheckOutDate = reservation.CheckOutDate.ToString("yyyy-MM-dd"),
            ReservationId = reservation.ReservationId
        });
    }

    public static List<int> GetReservationRoomIds(int id)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        List<int> room = connection.Query<int>(@"SELECT Room_Id 
                                                FROM Reservation_Room 
                                                WHERE Reservation_Id = @reservationId", 
                                                new {reservationId = id}).ToList();
        return room;
    }

    public static bool IsRoomFree(int roomid, DateTime newcheckin, DateTime newcheckout, int reservationIdToSkip)
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
        foreach (ReservationModel reservation in reservations)
        {
            if (reservation.Status == "cancelled")
            {
                continue;
            }
            if (reservation.ReservationId == reservationIdToSkip)
            {
                continue;
            }
            if (newcheckin >= reservation.CheckOutDate || newcheckout <= reservation.CheckInDate)
            {
                continue;
            }
            return false;
        }
        return true;
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

    public static void SwapRoom(int reservationid, int oldroomid, int newroomid)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        const string sql = @"UPDATE Reservation_Room
                            SET Room_Id = @newRoomId
                            WHERE Reservation_Id = @ReservationId 
                            AND Room_Id = @oldRoomId";

        connection.Execute(sql, new 
        {
            newRoomId = newroomid,
            ReservationId = reservationid,
            oldRoomId = oldroomid
        });
    }
}