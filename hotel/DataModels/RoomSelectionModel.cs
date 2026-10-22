// Carries the room selection between screens; this is not a database table.
public class RoomSelectionModel
{
    public List<ReservationRoomModel> SelectedRooms { get; set; } = new();
    public List<RoomModel> AvailableRooms { get; set; } = new();
    public List<RoomTypeModel> RoomTypes { get; set; } = new();
}
