public class FacilityServiceModel
{
    public int FacilityServiceId { get; set; }
    public int FacilityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
}
