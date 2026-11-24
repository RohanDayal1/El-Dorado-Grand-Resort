using Dapper;

public static class StatisticsAccess
{
    public static ResortStatisticsDataModel GetResortStatisticsData()
    {
        using var connection = DatabaseInitializer.Connect();
        using var results = connection.QueryMultiple(
            @"
            SELECT Status, CheckInDate, CheckOutDate, Adults, Children, TotalCost
            FROM Reservation;

            SELECT Role
            FROM Account;

            SELECT room.Room_Id AS RoomId,
                   r.Status AS ReservationStatus,
                   r.CheckInDate,
                   r.CheckOutDate
            FROM Room room
            LEFT JOIN Reservation_Room rr ON rr.Room_Id = room.Room_Id
            LEFT JOIN Reservation r ON r.Reservation_Id = rr.Reservation_Id
            WHERE room.Status = 'active';

            SELECT Rating
            FROM Review;

            SELECT f.Facility_Id AS FacilityId,
                   f.Name AS FacilityName,
                   fr.Facility_Reservation_Id AS FacilityReservationId,
                   fr.Status AS FacilityReservationStatus,
                   r.Status AS ReservationStatus,
                   fs.Facility_Service_Id AS ServiceId,
                   fs.Name AS ServiceName
            FROM Facility f
            LEFT JOIN Facility_Reservation fr ON fr.Facility_Id = f.Facility_Id
            LEFT JOIN Reservation r ON r.Reservation_Id = fr.Reservation_Id
            LEFT JOIN Facility_Service fs ON fs.Facility_Service_Id = fr.Facility_Service_Id
            ORDER BY f.Facility_Id, fr.Facility_Reservation_Id;"
        );

        return new ResortStatisticsDataModel
        {
            Reservations = results.Read<ReservationStatisticsData>().ToList(),
            Accounts = results.Read<AccountStatisticsData>().ToList(),
            Rooms = results.Read<RoomStatisticsData>().ToList(),
            Reviews = results.Read<ReviewStatisticsData>().ToList(),
            Facilities = results.Read<FacilityStatisticsData>().ToList(),
        };
    }
}
