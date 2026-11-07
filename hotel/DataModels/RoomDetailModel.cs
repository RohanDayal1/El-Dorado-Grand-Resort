public class RoomDetailModel
{
    public int RoomId { get; set; }
    public int Room_Type_Id { get; set; }
    public int RoomNumber { get; set; }
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public int PricePerNight { get; set; }
    public string? BedType { get; set; }
}