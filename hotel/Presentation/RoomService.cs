using System.Collections;
using System.Diagnostics;
using System.Net.Mail;

public static class RoomService
{
    
    public static void ShowSelection()
    {
        List<string> options = 
        [
            "Food & Drinks",
            "Housekeeping",
            "Extra Towels"
        ];

        int choice = ConsoleHelper.SelectionMenu(options, "Room Service", "Choose a Room Service:",null,"For any additional questions or requests, please contact our Service Desk. \nWe're happy to assist you!\n");
        switch (choice)
        {
            case 0:
            FoodnDrinks();
            break;

            case 1:
            Housekeeping();
            break;

            case 2:
            ExtraTowels();
            break;

            default:
            Menu.Start();
            break;
        }
    }

    public static void FoodnDrinks()
    {
        List<RoomServiceModel> AvailableFood = new List<RoomServiceModel>
        {
            new RoomServiceModel("Grilled chicken salad", "Grilled chicken, greens and house dressing", 0, 18.00),
            new RoomServiceModel("Classic cheeseburger", "Beef, aged cheese, lettuce and fries", 0, 19.50),
            new RoomServiceModel("Seasonal pasta", "Pasta with vegetables and parmesan", 0, 17.50),
            new RoomServiceModel("Chocolate cake", "Warm chocolate cake with vanilla ice cream", 0, 8.50),

            new RoomServiceModel("Bottled water", "Still or sparkling", 0, 3.50),
            new RoomServiceModel("Soft drink", "Cola, lemonade or iced tea", 0, 3.75),
            new RoomServiceModel("Coffee or tea", "Ask about today's tea selection", 0, 3.50)
        };

        List<string> details = AvailableFood.Select(item => $"{item.Description} | €{item.Price:F2}\n").ToList();

        while (true)
        {
            List<string> options = AvailableFood.Select(item => item.Quantity > 0 ? $"{item.Name}: (Added: {item.Quantity})" : $"{item.Name}:").ToList();
            int choice = ConsoleHelper.SelectionMenu(options, "FOOD & DRINKS", "Select the items you want", details, "  Press ENTER to add an item..\n");
            if(choice == -2)
            {
                break;
            }
            if (choice == -1)
            {
                break;
            }

            AvailableFood[choice].Quantity++;
        }

        List<RoomServiceModel> Order = AvailableFood.Where(item => item.Quantity > 0).ToList();

        if (Order.Count == 0)
        {
            return;
        }

        double totalprice = 0;
        string Summary = "";
        Console.WriteLine("Order Summary:\n");
        foreach(RoomServiceModel item in Order)
        {
            Summary += $"{item.Name} X {item.Quantity} || {item.Price:F2}\n";
            totalprice += item.Price;
        }

        List<string> actions = ["Yes", "No"];
        int action = ConsoleHelper.SelectionMenu(actions,"Order Summary", "Confirm ur order.",true,"Would you like to confirm ur order?\n" + Summary,null);
        
        switch (action)
        {
            case -2:
            Console.WriteLine("Succesfully confirmed your order. Your order will arrive soon to room ...");
            break;

            case 1:
            default:
            Console.WriteLine("Order cancled");
            break;


        }
    }

    public static void Housekeeping()
    {
        
    }

    public static void ExtraTowels()
    {
        
    }
}