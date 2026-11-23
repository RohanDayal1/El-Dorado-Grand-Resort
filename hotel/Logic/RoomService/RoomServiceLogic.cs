public class RoomServiceLogic
{
    public ReservationModel? GetActiveReservation(int userId)
    {
       return RoomServiceAccess.GetActiveReservation(userId);
    }

    public int GetRoomNumber(int reservationId)
    {
        return RoomServiceAccess.GetRoomNumber(reservationId);
    }
    
    //getfood ? mischien
    //housekeeping
    //Extra towels
    //valideren
    //prijs uitrekenen
    //order plaatsen
    //Get orders
}