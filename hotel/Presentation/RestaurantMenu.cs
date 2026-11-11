using System.Globalization;

public static class RestaurantMenu
{
    private const int NameWidth = 24;
    private const int DescriptionWidth = 44;
    private const int PriceWidth = 10;

    private static readonly (string Name, string Description, decimal Price)[] Food =
    {
        ("Grilled chicken salad", "Grilled chicken, greens and house dressing", 18.00m),
        ("Classic cheeseburger", "Beef, aged cheese, lettuce and fries", 19.50m),
        ("Seasonal pasta", "Pasta with vegetables and parmesan", 17.50m),
        ("Chocolate cake", "Warm chocolate cake with vanilla ice cream", 8.50m),
    };

    private static readonly (string Name, string Description, decimal Price)[] Drinks =
    {
        ("Bottled water", "Still or sparkling", 3.50m),
        ("Soft drink", "Cola, lemonade or iced tea", 3.75m),
        ("Coffee or tea", "Ask about today's tea selection", 3.50m),
    };

    public static void ShowMenuItems()
    {
        ShowSection("FOOD", "Dish", Food);
        ShowSection("DRINKS", "Drink", Drinks);
    }

    private static void ShowSection(
        string title,
        string itemColumn,
        (string Name, string Description, decimal Price)[] items
    )
    {
        Console.WriteLine($"  {title}");
        Console.WriteLine($"  +{new string('-', NameWidth + 2)}+{new string('-', DescriptionWidth + 2)}+{new string('-', PriceWidth + 2)}+");
        Console.WriteLine(
            $"  | {itemColumn.ToUpperInvariant(),-NameWidth} | {"DESCRIPTION",-DescriptionWidth} | {"PRICE",PriceWidth} |"
        );
        Console.WriteLine($"  +{new string('-', NameWidth + 2)}+{new string('-', DescriptionWidth + 2)}+{new string('-', PriceWidth + 2)}+");
        foreach (var item in items)
        {
            string price = item.Price.ToString("0.00", CultureInfo.InvariantCulture);
            Console.WriteLine(
                $"  | {item.Name,-NameWidth} | {item.Description,-DescriptionWidth} | {"€" + price,PriceWidth} |"
            );
        }

        Console.WriteLine($"  +{new string('-', NameWidth + 2)}+{new string('-', DescriptionWidth + 2)}+{new string('-', PriceWidth + 2)}+");
        Console.WriteLine();
    }
}
