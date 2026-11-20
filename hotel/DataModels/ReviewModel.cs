public class ReviewModel
{
    public int ReviewId { get; set; }
    public int ReservationId { get; set; }
    public int Rating { get; set; }
    public string? Feedback { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
