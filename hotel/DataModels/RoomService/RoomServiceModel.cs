public class RoomServiceModel
{
    public string Name {get; set;}
    public string Description {get; set;}
    public int Quantity {get; set;}
    public double Price {get; set;}

    public RoomServiceModel(string name, string description , int quantity, double price)
    {
        Name = name;
        Description = description;
        Quantity = quantity;
        Price = price;
    }
    
}