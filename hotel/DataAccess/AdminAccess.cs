using Microsoft.Data.Sqlite;
using Dapper;
using System.Data;

public static class AdminAccess
{
    public static void ViewAllGuest()
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
        foreach (var guest in AllGuests)
        {
            Console.WriteLine(@$"Userid: {guest.UserId}, First name: {guest.FirstName}, Last name: {guest.LastName}, Email: {guest.Email}, Phone: {guest.Phone}, Date of birth: {guest.DateOfBirth:yyyy-MM-dd}");
        }
    }

    public static void ViewAllReservation()
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
        foreach (var reservation in AllReservation)
        {
            Console.WriteLine($"Reservationid: {reservation.ReservationId}, Reservationcode: {reservation.ReservationCode}, UserId: {reservation.UserId}, CheckInDate: {reservation.CheckInDate:yyyy-MM-dd}, CheckOutDate: {reservation.CheckOutDate:yyyy-MM-dd}, TotalCost: {reservation.TotalCost}, Status: {reservation.Status}");
        }
    }

    public static void EditReservation()
    {
        Console.Write("Reservation ID: ");
        string? id = Console.ReadLine();

        ReservationModel? currentReservation = null;
        if (int.TryParse(id, out int reservationId))
        {
            currentReservation = GetReservation(reservationId);
        }
        else
        {
            Console.WriteLine("not a valid number!");
            return;
        }
        if (currentReservation == null)
        {
            Console.WriteLine("reservation doesn't exist");
            return;
        }

        Console.Write("New Check-in-Date: ");
        string? checkInDate = Console.ReadLine();
        if (!DateTime.TryParse(checkInDate, out DateTime newCheckInDate))
        {
            Console.WriteLine("not a valid date");
            return;
        }

        Console.Write("New Check-Out-Date: ");
        string? checkOutDate = Console.ReadLine();
        if (!DateTime.TryParse(checkOutDate, out DateTime newCheckOutDate))
        {
            Console.WriteLine("not a valid date");
            return;
        }

        if (!(newCheckOutDate > newCheckInDate))
        {
            Console.WriteLine("invalid dates");
            return;
        }

        Dictionary<int, int> RoomSwap = new();
        List<int> ids = GetRooms(reservationId).ToList();
        foreach (var roomId in ids)
        {
            bool foundOtherRoom = false;
            
            bool currentRoomisFree = IsRoomFree(roomId, newCheckInDate, newCheckOutDate, reservationId);
            if (!currentRoomisFree)
            {
                List<int> otherRooms = GetRoomOfSameType(roomId).ToList();

                foreach (int otherRoomId in otherRooms)
                {
                    if (ids.Contains(otherRoomId) || RoomSwap.ContainsValue(otherRoomId))
                    {
                        continue;
                    }
                    bool isOtherRoomFree = IsRoomFree(otherRoomId, newCheckInDate, newCheckOutDate, reservationId);
                    if (isOtherRoomFree)
                    {
                        foundOtherRoom = true;
                        RoomSwap.Add(roomId, otherRoomId);
                        break;
                    }
                }
                if (!foundOtherRoom)
                {
                    Console.WriteLine("Room isn't free");
                    return;
                }
            }
        }

        currentReservation.CheckInDate = newCheckInDate;
        currentReservation.CheckOutDate = newCheckOutDate;

        Console.WriteLine($"{currentReservation.ReservationId} {currentReservation.ReservationCode} {currentReservation.CheckInDate:yyyy-MM-dd} {currentReservation.CheckOutDate:yyyy-MM-dd}");
        UpdateReservationToDb(currentReservation);

        foreach (KeyValuePair<int, int> pair in RoomSwap)
        {
            SwapRoom(reservationId, pair.Key, pair.Value);
        }
    }

    public static void CancelReservation(int reservationid)
    {
        ReservationModel? reservation = GetReservation(reservationid);
        if (reservation == null)
        {
            Console.WriteLine("Reservation doesn't exist");
            return;
        }
        if (reservation.Status == "cancelled")
        {
            Console.WriteLine("Reservation is already cancelled");
            return;
        }

        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        const string sql = @"UPDATE Reservation SET Status = 'cancelled' WHERE Reservation_Id = @ReservationId";
        connection.Execute(sql, new {ReservationId = reservationid});

        Console.WriteLine("Reservation cancelled");
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

    public static List<int> GetRooms(int id)
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