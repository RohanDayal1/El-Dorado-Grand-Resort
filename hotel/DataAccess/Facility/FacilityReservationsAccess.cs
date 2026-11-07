using Dapper;
using Microsoft.Data.Sqlite;

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

    public static int InsertReservation(FacilityReservationModel booking)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        int reservationId = InsertReservation(connection, transaction, booking);
        transaction.Commit();
        return reservationId;
    }

    // Keep the current-data reads and insert in one transaction. Logic supplies validation.
    public static FacilityReservationModel? InsertReservation(
        int reservationId,
        int serviceId,
        DateTime date,
        Func<ReservationModel?, FacilityModel?, FacilityServiceModel?,
            List<FacilityReservationModel>, FacilityReservationModel?> createBooking
    )
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        using var transaction = connection.BeginTransaction(deferred: false);

        ReservationModel? stay;
        FacilityModel? facility;
        FacilityServiceModel? service;
        List<FacilityReservationModel> bookings;
        using (var results = connection.QueryMultiple(
            @"
            SELECT Reservation_Id AS ReservationId, ReservationCode, User_Id AS UserId,
                   CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status
            FROM Reservation WHERE Reservation_Id = @ReservationId;

            SELECT Facility_Id AS FacilityId, Name, Description, Capacity,
                   '0001-01-01 ' || OpeningHours AS OpeningHours,
                   '0001-01-01 ' || ClosingHours AS ClosingHours, Location
            FROM Facility
            WHERE Facility_Id = (SELECT Facility_Id FROM Facility_Service
                                 WHERE Facility_Service_Id = @ServiceId);

            SELECT Facility_Service_Id AS FacilityServiceId, Facility_Id AS FacilityId,
                   Name, Description, DurationMinutes
            FROM Facility_Service WHERE Facility_Service_Id = @ServiceId;

            SELECT Facility_Reservation_Id AS FacilityReservationId, Account_Id AS AccountId,
                   Reservation_Id AS ReservationId, Facility_Id AS FacilityId,
                   Facility_Service_Id AS FacilityServiceId, ReservationDate,
                   ReservationDate || ' ' || StartTime AS StartTime,
                   ReservationDate || ' ' || EndTime AS EndTime, PartySize, Status
            FROM Facility_Reservation
            WHERE Facility_Id = (SELECT Facility_Id FROM Facility_Service
                                 WHERE Facility_Service_Id = @ServiceId)
              AND ReservationDate = @ReservationDate AND Status = 'confirmed';",
            new { ReservationId = reservationId, ServiceId = serviceId,
                  ReservationDate = date.ToString("yyyy-MM-dd") },
            transaction
        ))
        {
            stay = results.ReadFirstOrDefault<ReservationModel>();
            facility = results.ReadFirstOrDefault<FacilityModel>();
            service = results.ReadFirstOrDefault<FacilityServiceModel>();
            bookings = results.Read<FacilityReservationModel>().ToList();
        }

        var booking = createBooking(stay, facility, service, bookings);
        if (booking == null)
            return null;

        booking.FacilityReservationId = InsertReservation(connection, transaction, booking);
        transaction.Commit();
        return booking;
    }

    private static int InsertReservation(
        SqliteConnection connection,
        SqliteTransaction transaction,
        FacilityReservationModel booking
    )
    {
        int reservationId = connection.QuerySingle<int>(
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
            },
            transaction
        );

        return reservationId;
    }
}
