public class ReservationDetailModel
{
    public int ReservationId { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
    public string? FirstName {get; set; }
    public string? LastName {get; set; } 
    public int Adults { get; set; }
    public int Children { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalCost { get; set; }
    public string? Status { get; set; }
}
