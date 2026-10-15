using Microsoft.Data.Sqlite;
using Dapper;

public static class AdminAccess
{
    private static readonly string ConnectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "DataSources", "el_dorado.db")};Foreign Keys=True";
    public static SqliteConnection Connect() => new SqliteConnection(ConnectionString);

    public static void ViewAllGuest()
    {
        using var connection = Connect();
        connection.Open();
        var AllGuests = connection.Query<UserModel>("SELECT * FROM User").ToList();
        foreach (var guest in AllGuests)
        {
            Console.WriteLine($"first name: {guest.FirstName}, last name: {guest.LastName}, Email: {guest.Email}, Phone: {guest.Phone}, Date of birth: {guest.DateOfBirth}");
        }
    }

    public static void ViewAllReservation()
    {
        using var connection = Connect();
        connection.Open();
        var AllReservation = connection.Query<ReservationModel>("SELECT * FROM Reservation").ToList();
        foreach (var reservation in AllReservation)
        {
            Console.WriteLine($"ReservationId = {reservation.ReservationId}, ReservationCode  = {reservation.ReservationCode}, UserId = {reservation.UserId}, CheckInDate = {reservation.CheckInDate}, CheckOutDate = {reservation.CheckOutDate}, TotalCost = {reservation.TotalCost}, Status = {reservation.Status}");
        }
    }

    public static void UpdateReservation(ReservationModel reservation)
    {
        using var connection = Connect();
        connection.Open();

        // string? status = connection.QueryFirstOrDefault<string>("SELECT Status FROM Reservation WHERE ReservationId = @ReservationId", new {ReservationId = reservation.ReservationId});
        // if (status != "Confirmed")
        // {
        //     Console.WriteLi
        // }

        string sql = @"UPDATE Reservation 
                    SET CheckInDate = @CheckInDate, 
                    CheckOutDate = @CheckOutDate, 
                    TotalCost = @TotalCost, 
                    Status = @Status 
                    WHERE ReservationId = @ReservationId";
        connection.Execute(sql, reservation);
    }

    public static void Delete(int id)
    {
        using var connection = Connect();
        connection.Open();

        var reservation = connection.QueryFirstOrDefault<ReservationModel>("SELECT ReservationId FROM Reservation WHERE ReservationId = @Id", new {Id = id});
        if (reservation != null)
        {
            connection.Execute("DELETE FROM Reservation WHERE ReservationId = @ReservationId", new {reservationId = id});
            Console.WriteLine("Succesfully deleted the reservation");
        }
        else
        {
            Console.WriteLine("reservation doesn't exist");
        }
    }

    // public static void Write(UserModel book)
    // {
    //     string sql = $"INSERT INTO {_table} (author, title) VALUES (@Author, @Title)";
    //     _connection.Execute(sql, book);
    // }
}