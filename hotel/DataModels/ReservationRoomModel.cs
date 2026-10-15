public class ReservationRoomModel
{
    public int ReservationId { get; set; }
    public int RoomId { get; set; }
    public int Adults { get; set; } = 1;
    public int Children { get; set; }
}
