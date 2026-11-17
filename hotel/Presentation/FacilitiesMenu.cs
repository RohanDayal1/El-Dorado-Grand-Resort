public static class FacilitiesMenu
{
    public static void Start(AccountModel? account = null)
    {
        var facilitiesLogic = new FacilitiesLogic();
        var hotelFacilities = facilitiesLogic.GetFacilities();

        if (hotelFacilities.Count == 0)
        {
            ConsoleHelper.ShowHeader("OUR FACILITIES");
            Console.WriteLine("  No facilities are available at the moment.");
            ConsoleHelper.WaitForEnter();
            return;
        }

        while (true)
        {
            List<string> facilityNames = new List<string>();

            foreach (FacilityModel facility in hotelFacilities)
            {
                facilityNames.Add(facility.Name);
            }

            int choice = ConsoleHelper.SelectionMenu(facilityNames, "FACILITIES", "Choose a facility");

            if (choice == -1)
                return;

            FacilityModel selectedFacility = hotelFacilities[choice];

            ConsoleHelper.ShowHeader(selectedFacility.Name.ToUpper());
            ShowFacilityDetails(selectedFacility);

            if (selectedFacility.Name.Equals("Restaurant", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                RestaurantMenu.ShowMenuItems();
                Console.WriteLine();
                Console.Write("  Press Enter to return to the facilities...");
                Console.ReadLine();
                ConsoleHelper.ClearScreen();
                continue;
            }

            var facilityServices = facilitiesLogic.GetServices(selectedFacility.FacilityId);

            if (facilityServices.Count == 0)
            {
                Console.WriteLine("\n  There are no services to book at this facility.");
                Console.Write("\n  Press Enter to return to the facilities...");
                Console.ReadLine();
                continue;
            }

            var selectedService = ChooseFacilityService(facilityServices);

            if (selectedService == null)
                continue;

            if (account == null)
            {
                Console.WriteLine("\n  Please log in from the home menu to book this service.");
                ConsoleHelper.WaitForEnter();
                return;
            }

            FacilityReservationMenu.Start(account, selectedFacility, selectedService);
            return;
        }
    }

    private static void ShowFacilityDetails(FacilityModel selectedFacility)
    {
        if (!string.IsNullOrWhiteSpace(selectedFacility.Description))
            Console.WriteLine($"  {selectedFacility.Description}");

        if (!string.IsNullOrWhiteSpace(selectedFacility.Location))
            Console.WriteLine($"  Location: {selectedFacility.Location}");

        if (selectedFacility.OpeningHours.HasValue && selectedFacility.ClosingHours.HasValue)
            Console.WriteLine(
                $"  Opening hours: {selectedFacility.OpeningHours:HH:mm} - {selectedFacility.ClosingHours:HH:mm}"
            );
    }

    private static FacilityServiceModel? ChooseFacilityService(
        List<FacilityServiceModel> facilityServices
    )
    {
        List<string> serviceNames = new List<string>();

        foreach (FacilityServiceModel service in facilityServices)
        {
            serviceNames.Add(service.Name);
        }

        int choice = ConsoleHelper.SelectionMenu(serviceNames, "AVAILABLE SERVICES", "Choose a service");

        if (choice == -1)
            return null;

        return facilityServices[choice];
    }
}
