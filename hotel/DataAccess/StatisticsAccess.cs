using Dapper;

public static class StatisticsAccess
{
    public static (
        List<ReservationModel> Reservations,
        List<AccountModel> Accounts,
        List<RoomModel> Rooms,
        List<ReservationRoomModel> ReservationRooms,
        List<ReviewModel> Reviews,
        List<FacilityModel> Facilities,
        List<FacilityReservationModel> FacilityReservations,
        List<FacilityServiceModel> FacilityServices
    ) GetResortStatisticsData()
    {
        using var connection = DatabaseInitializer.Connect();
        using var results = connection.QueryMultiple(
            @"
            SELECT Reservation_Id AS ReservationId, ReservationCode, User_Id AS UserId,
                   CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status
            FROM Reservation;

            SELECT Account_Id AS Id, User_Id AS UserId, Role
            FROM Account;

            SELECT Room_Id AS RoomId, Room_Type_Id, RoomNumber, Status
            FROM Room;

            SELECT Reservation_Id AS ReservationId, Room_Id AS RoomId
            FROM Reservation_Room;

            SELECT Review_Id AS ReviewId, Reservation_Id AS ReservationId,
                   Rating, Feedback, CreatedAt
            FROM Review;

            SELECT Facility_Id AS FacilityId, Name
            FROM Facility;

            SELECT Facility_Reservation_Id AS FacilityReservationId,
                   Reservation_Id AS ReservationId, Facility_Id AS FacilityId,
                   Facility_Service_Id AS FacilityServiceId, Status
            FROM Facility_Reservation;

            SELECT Facility_Service_Id AS FacilityServiceId, Facility_Id AS FacilityId, Name
            FROM Facility_Service;"
        );

        return (
            results.Read<ReservationModel>().ToList(),
            results.Read<AccountModel>().ToList(),
            results.Read<RoomModel>().ToList(),
            results.Read<ReservationRoomModel>().ToList(),
            results.Read<ReviewModel>().ToList(),
            results.Read<FacilityModel>().ToList(),
            results.Read<FacilityReservationModel>().ToList(),
            results.Read<FacilityServiceModel>().ToList()
        );
    }
}
