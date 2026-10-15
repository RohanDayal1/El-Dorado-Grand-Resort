public class RoomTypeModel
{
    public int Room_Type_Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string? BedType { get; set; }
}
