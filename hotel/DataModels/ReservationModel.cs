public class ReservationModel
{
    public int ReservationId { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalCost { get; set; }
    public string Status { get; set; } = "confirmed";
}
