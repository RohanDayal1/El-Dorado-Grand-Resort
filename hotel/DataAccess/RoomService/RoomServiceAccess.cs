using Dapper;
using Microsoft.Data.Sqlite;

public static class RoomServiceAcces
{
    private static readonly SqliteConnection _connection = new($"Data Source=DataSources/el_dorado.db");

    public static ReservationModel? GetActiveReservation(int userId)
    {
        string sql = "SELECT Reservation_Id AS ReservationId, ReservationCode, User_Id AS UserId, CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status FROM Reservation WHERE User_Id = @UserId AND Status = 'confirmed'";

        return _connection.QueryFirstOrDefault<ReservationModel>(sql, new { UserId = userId });
    }
    
}