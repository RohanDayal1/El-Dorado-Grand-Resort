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

    public static int GetRoomNumber(int reservationId)
    {
        string sql = "SELECT Room_Id FROM Reservation_Room WHERE Reservation_Id = @ReservationId";

        int roomId = _connection.QueryFirstOrDefault<int>(sql, new { ReservationId = reservationId });

        string roomSql = "SELECT RoomNumber FROM Room WHERE Room_Id = @RoomId";

        return _connection.QueryFirstOrDefault<int>(roomSql, new { RoomId = roomId });
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