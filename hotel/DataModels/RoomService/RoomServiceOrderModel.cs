public class RoomServiceOrderModel
{
    public int Id { get; set; }
    public int ReservationId { get; set; }
    public int RoomNumber { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal Total { get; set; }
    public List<RoomServiceOrderItemModel> Items { get; set; } = new List<RoomServiceOrderItemModel>();
    
}