using Dapper;

public static class FacilityReservationsAccess
{
    public static List<FacilityReservationModel> GetConfirmedByFacilityAndDate(
        int facilityId,
        DateTime date
    )
    {
        using var connection = DatabaseInitializer.Connect();

        return connection
            .Query<FacilityReservationModel>(
                @"
            SELECT Facility_Reservation_Id AS FacilityReservationId, Account_Id AS AccountId,
                   Reservation_Id AS ReservationId, Facility_Id AS FacilityId,
                   Facility_Service_Id AS FacilityServiceId, ReservationDate,
                   ReservationDate || ' ' || StartTime AS StartTime,
                   ReservationDate || ' ' || EndTime AS EndTime, PartySize, Status
            FROM Facility_Reservation
            WHERE Facility_Id = @FacilityId
              AND ReservationDate = @ReservationDate
              AND Status = 'confirmed'
            ORDER BY StartTime, Facility_Reservation_Id;",
                new
                {
                    FacilityId = facilityId,
                    ReservationDate = date.ToString("yyyy-MM-dd"),
                }
            )
            .ToList();
    }

    public static List<FacilityReservationModel> GetByUserId(int userId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection
            .Query<FacilityReservationModel>(
                @"
            SELECT fr.Facility_Reservation_Id AS FacilityReservationId, fr.Account_Id AS AccountId,
                   fr.Reservation_Id AS ReservationId, fr.Facility_Id AS FacilityId,
                   fr.Facility_Service_Id AS FacilityServiceId, fr.ReservationDate,
                   fr.ReservationDate || ' ' || fr.StartTime AS StartTime,
                   fr.ReservationDate || ' ' || fr.EndTime AS EndTime, fr.PartySize, fr.Status,
                   f.Name AS FacilityName, fs.Name AS ServiceName
            FROM Facility_Reservation fr
            JOIN Reservation r ON r.Reservation_Id = fr.Reservation_Id
            JOIN Facility f ON f.Facility_Id = fr.Facility_Id
            JOIN Facility_Service fs ON fs.Facility_Service_Id = fr.Facility_Service_Id
            WHERE r.User_Id = @UserId
            ORDER BY fr.ReservationDate, fr.StartTime, fr.Facility_Reservation_Id;",
                new { UserId = userId }
            )
            .ToList();
    }

    public static List<FacilityReservationModel> GetConfirmedByReservationId(int reservationId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection
            .Query<FacilityReservationModel>(
                @"
            SELECT Facility_Reservation_Id AS FacilityReservationId, Account_Id AS AccountId,
                   Reservation_Id AS ReservationId, Facility_Id AS FacilityId,
                   Facility_Service_Id AS FacilityServiceId, ReservationDate,
                   ReservationDate || ' ' || StartTime AS StartTime,
                   ReservationDate || ' ' || EndTime AS EndTime, PartySize, Status
            FROM Facility_Reservation
            WHERE Reservation_Id = @ReservationId
              AND Status = 'confirmed'
            ORDER BY ReservationDate, StartTime, Facility_Reservation_Id;",
                new { ReservationId = reservationId }
            )
            .ToList();
    }

    public static List<FacilityReservationModel> GetByReservationId(int reservationId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.Query<FacilityReservationModel>(
            @"
            SELECT fr.Facility_Reservation_Id AS FacilityReservationId,
                   fr.Account_Id AS AccountId,
                   fr.Reservation_Id AS ReservationId,
                   fr.Facility_Id AS FacilityId,
                   fr.Facility_Service_Id AS FacilityServiceId,
                   fr.ReservationDate,
                   fr.ReservationDate || ' ' || fr.StartTime AS StartTime,
                   fr.ReservationDate || ' ' || fr.EndTime AS EndTime,
                   fr.PartySize,
                   fr.Status,
                   f.Name AS FacilityName,
                   fs.Name AS ServiceName
            FROM Facility_Reservation fr
            JOIN Facility f ON f.Facility_Id = fr.Facility_Id
            JOIN Facility_Service fs ON fs.Facility_Service_Id = fr.Facility_Service_Id
            WHERE fr.Reservation_Id = @ReservationId
            ORDER BY fr.ReservationDate, fr.StartTime, fr.Facility_Reservation_Id;",
            new { ReservationId = reservationId }
        ).ToList();
    }

    public static int InsertReservation(FacilityReservationModel booking)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        return connection.QuerySingle<int>(
            @"
            INSERT INTO Facility_Reservation
                (Account_Id, Reservation_Id, Facility_Id, Facility_Service_Id,
                 ReservationDate, StartTime, EndTime, PartySize, Status)
            VALUES
                (@AccountId, @ReservationId, @FacilityId, @FacilityServiceId,
                 @ReservationDate, @StartTime, @EndTime, @PartySize, @Status);
            SELECT last_insert_rowid();",
            new
            {
                booking.AccountId,
                booking.ReservationId,
                booking.FacilityId,
                booking.FacilityServiceId,
                ReservationDate = booking.ReservationDate.ToString("yyyy-MM-dd"),
                StartTime = booking.StartTime.ToString("HH:mm"),
                EndTime = booking.EndTime.ToString("HH:mm"),
                booking.PartySize,
                booking.Status,
            }
        );
    }
}
