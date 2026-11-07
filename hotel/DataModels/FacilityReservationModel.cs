public class FacilityReservationModel
{
    public int FacilityReservationId { get; set; }
    public int AccountId { get; set; }
    public int ReservationId { get; set; }
    public int FacilityId { get; set; }
    public int FacilityServiceId { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int PartySize { get; set; }
    public string Status { get; set; } = "confirmed";
    public string FacilityName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
}
