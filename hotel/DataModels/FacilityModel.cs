public class FacilityModel
{
    public int FacilityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public DateTime? OpeningHours { get; set; }
    public DateTime? ClosingHours { get; set; }
    public string? Location { get; set; }
}
