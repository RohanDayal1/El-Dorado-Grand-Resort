public class RoomTypeModel
{
    public int RoomTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string? BedType { get; set; }
}
