public class FacilityReservationDetailModel
{
    public string FacilityName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string? FacilityDescription { get; set; }
    public string? ServiceDescription { get; set; }
    public DateTime ReservationDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int PartySize { get; set; }
    public string Status { get; set; } = string.Empty;
}
