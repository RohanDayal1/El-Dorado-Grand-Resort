using Dapper;

public static class StatisticsAccess
{
    public static ResortStatisticsModel GetResortStatistics()
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.QuerySingle<ResortStatisticsModel>(@"
            SELECT
                (SELECT COUNT(*) FROM Reservation) AS TotalReservations,
                (SELECT COUNT(*) FROM Reservation WHERE Status = 'confirmed') AS ActiveReservations,
                (SELECT COUNT(*) FROM Reservation WHERE Status = 'cancelled') AS CancelledReservations,
                (SELECT COUNT(*) FROM Account WHERE Role = 'guest') AS RegisteredGuests;");
    }
}
