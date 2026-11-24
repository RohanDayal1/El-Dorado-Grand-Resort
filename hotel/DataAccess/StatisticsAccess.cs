using Dapper;

public static class StatisticsAccess
{
    public static ResortStatisticsModel GetResortStatistics(DateTime today)
    {
        using var connection = DatabaseInitializer.Connect();
        string todayText = today.ToString("yyyy-MM-dd");

        ResortStatisticsModel statistics = connection.QuerySingle<ResortStatisticsModel>(
            @"
            SELECT
                (SELECT COUNT(*) FROM Reservation) AS TotalReservations,
                (SELECT COUNT(*) FROM Reservation WHERE Status = 'confirmed') AS ConfirmedReservations,
                (SELECT COUNT(*) FROM Reservation WHERE Status = 'cancelled') AS CancelledReservations,
                (SELECT COUNT(*) FROM Account WHERE Role = 'guest') AS RegisteredGuests,
                (SELECT COUNT(*) FROM Room WHERE Status = 'active') AS ActiveRooms,
                (SELECT COUNT(DISTINCT rr.Room_Id)
                 FROM Reservation_Room rr
                 JOIN Reservation r ON r.Reservation_Id = rr.Reservation_Id
                 JOIN Room room ON room.Room_Id = rr.Room_Id
                 WHERE r.Status = 'confirmed'
                   AND room.Status = 'active'
                   AND r.CheckInDate <= @Today
                   AND r.CheckOutDate > @Today) AS OccupiedRoomsToday,
                (SELECT COALESCE(SUM(r.Adults + r.Children), 0)
                 FROM Reservation r
                 WHERE r.Status = 'confirmed'
                   AND r.CheckInDate <= @Today
                   AND r.CheckOutDate > @Today) AS GuestsStayingToday,
                (SELECT COALESCE(SUM(r.Adults + r.Children), 0)
                 FROM Reservation r
                 WHERE r.Status = 'confirmed'
                   AND r.CheckInDate = @Today) AS ArrivingGuestsToday,
                (SELECT COALESCE(SUM(r.Adults + r.Children), 0)
                 FROM Reservation r
                 WHERE r.Status = 'confirmed'
                   AND r.CheckOutDate = @Today) AS DepartingGuestsToday,
                (SELECT COALESCE(SUM(TotalCost), 0)
                 FROM Reservation
                 WHERE Status = 'confirmed') AS ConfirmedBookingValue,
                (SELECT COALESCE(AVG(TotalCost), 0)
                 FROM Reservation
                 WHERE Status = 'confirmed') AS AverageConfirmedBookingValue,
                (SELECT COUNT(*) FROM Review) AS TotalReviews,
                (SELECT AVG(Rating) FROM Review) AS AverageReviewRating,
                (SELECT COUNT(*)
                 FROM Facility_Reservation fr
                 JOIN Reservation r ON r.Reservation_Id = fr.Reservation_Id
                 WHERE fr.Status = 'confirmed'
                   AND r.Status = 'confirmed') AS ConfirmedFacilityBookings,
                (SELECT fs.Name
                 FROM Facility_Reservation fr
                 JOIN Reservation r ON r.Reservation_Id = fr.Reservation_Id
                 JOIN Facility_Service fs ON fs.Facility_Service_Id = fr.Facility_Service_Id
                 WHERE fr.Status = 'confirmed'
                   AND r.Status = 'confirmed'
                 GROUP BY fs.Facility_Service_Id, fs.Name
                 ORDER BY COUNT(*) DESC, fs.Name
                 LIMIT 1) AS MostBookedServiceName,
                (SELECT COUNT(*)
                 FROM Facility_Reservation fr
                 JOIN Reservation r ON r.Reservation_Id = fr.Reservation_Id
                 WHERE fr.Status = 'confirmed'
                   AND r.Status = 'confirmed'
                 GROUP BY fr.Facility_Service_Id
                 ORDER BY COUNT(*) DESC, fr.Facility_Service_Id
                 LIMIT 1) AS MostBookedServiceCount;",
            new { Today = todayText }
        );

        statistics.AvailableRoomsToday = Math.Max(
            0,
            statistics.ActiveRooms - statistics.OccupiedRoomsToday
        );
        statistics.OccupancyPercentage = statistics.ActiveRooms == 0
            ? 0
            : statistics.OccupiedRoomsToday * 100m / statistics.ActiveRooms;

        statistics.CancellationPercentage = statistics.TotalReservations == 0
            ? 0
            : statistics.CancelledReservations * 100m / statistics.TotalReservations;

        statistics.AverageReviewRating ??= 0;
        statistics.ReviewsByRating = connection.Query<RatingStatistics>(
            @"
            SELECT ratings.Rating,
                   COUNT(review.Review_Id) AS ReviewCount
            FROM (
                SELECT 1 AS Rating
                UNION ALL SELECT 2
                UNION ALL SELECT 3
                UNION ALL SELECT 4
                UNION ALL SELECT 5
            ) ratings
            LEFT JOIN Review review ON review.Rating = ratings.Rating
            GROUP BY ratings.Rating
            ORDER BY ratings.Rating;"
        ).ToList();

        statistics.BookingsByFacility = connection.Query<FacilityBookingStatistics>(
            @"
            SELECT f.Name AS FacilityName,
                   COUNT(fr.Facility_Reservation_Id) AS ConfirmedBookings
            FROM Facility f
            LEFT JOIN Facility_Reservation fr
              ON fr.Facility_Id = f.Facility_Id
             AND fr.Status = 'confirmed'
             AND EXISTS (
                 SELECT 1
                 FROM Reservation r
                 WHERE r.Reservation_Id = fr.Reservation_Id
                   AND r.Status = 'confirmed'
             )
            GROUP BY f.Facility_Id, f.Name
            ORDER BY f.Name;"
        ).ToList();

        return statistics;
    }
}
