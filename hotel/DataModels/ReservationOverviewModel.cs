public class ReservationOverviewModel : ReservationModel
{
    public string GuestName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<ReservationRoomDetailModel> Rooms { get; set; } = new();
    public List<FacilityReservationDetailModel> Facilities { get; set; } = new();
}
