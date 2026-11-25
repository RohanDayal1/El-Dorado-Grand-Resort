using Dapper;
using Microsoft.Data.Sqlite;

public static class RoomServiceAccess
{
    private static readonly SqliteConnection _connection = new($"Data Source=DataSources/el_dorado.db");

    public static ReservationModel? GetActiveReservation(int userId)
    {
        string sql = "SELECT Reservation_Id AS ReservationId, ReservationCode, User_Id AS UserId, CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status FROM Reservation WHERE User_Id = @UserId AND Status = 'confirmed'";
        List<ReservationModel> reservations = _connection.Query<ReservationModel>(sql, new { UserId = userId }).ToList();

        foreach (ReservationModel reservation in reservations)
        {
            if (reservation.CheckInDate.Date <= DateTime.Today && reservation.CheckOutDate.Date > DateTime.Today)
                return reservation;
        }

        return null;
    }

    public static List<int> GetRoomNumbers(int reservationId)
    {
        string sql = "SELECT Room_Id FROM Reservation_Room WHERE Reservation_Id = @ReservationId";
        List<int> roomIds = _connection.Query<int>(sql, new { ReservationId = reservationId }).ToList();

        List<int> roomNumbers = new List<int>();

        foreach (int roomId in roomIds)
        {
            string roomSql = "SELECT RoomNumber FROM Room WHERE Room_Id = @RoomId";
            int roomNumber = _connection.QueryFirstOrDefault<int>(roomSql, new { RoomId = roomId });
            roomNumbers.Add(roomNumber);
        }

        return roomNumbers;
    }

    public static int SaveOrder(RoomServiceOrderModel order)
    {
        string sql = "INSERT INTO RoomServiceOrder (Reservation_Id, RoomNumber, OrderDate, Status, TotalPrice) VALUES (@ReservationId, @RoomNumber, @Date, @Status, @Total); SELECT last_insert_rowid()";

        int orderId = _connection.ExecuteScalar<int>(sql, order);

        foreach (RoomServiceOrderItemModel item in order.Items)
        {
            string itemSql = "INSERT INTO RoomServiceOrderItem (RoomServiceOrder_Id, Name, Quantity, Price) VALUES (@OrderId, @Name, @Quantity, @Price)";

            item.OrderId = orderId;
            _connection.Execute(itemSql, item);
        }

        return orderId;
    }

    public static List<RoomServiceOrderModel> GetOrders(int reservationId)
    {
        string sql = "SELECT RoomServiceOrder_Id AS Id, Reservation_Id AS ReservationId, RoomNumber, OrderDate AS Date, Status, TotalPrice AS Total FROM RoomServiceOrder WHERE Reservation_Id = @ReservationId";

        List<RoomServiceOrderModel> orders = _connection.Query<RoomServiceOrderModel>(sql, new { ReservationId = reservationId }).ToList();

        foreach (RoomServiceOrderModel order in orders)
        {
            string itemSql = "SELECT RoomServiceOrderItem_Id AS Id, RoomServiceOrder_Id AS OrderId, Name, Quantity, Price FROM RoomServiceOrderItem WHERE RoomServiceOrder_Id = @OrderId";

            order.Items = _connection.Query<RoomServiceOrderItemModel>(itemSql, new { OrderId = order.Id }).ToList();
        }

        return orders;
    }
    
}