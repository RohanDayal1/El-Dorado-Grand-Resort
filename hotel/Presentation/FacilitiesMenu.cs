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
            ConsoleHelper.ShowHeader("OUR FACILITIES");
            for (int i = 0; i < hotelFacilities.Count; i++)
                Console.WriteLine($"  [{i + 1}] {hotelFacilities[i].Name}");

            var selectedFacility = ChooseFacility(hotelFacilities);
            if (selectedFacility == null)
                return;

            ConsoleHelper.ShowHeader(selectedFacility.Name.ToUpper());
            ShowFacilityDetails(selectedFacility);
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

    private static FacilityModel? ChooseFacility(List<FacilityModel> hotelFacilities)
    {
        while (true)
        {
            int? choice = ReservationInput.ReadInt("Choose a facility: ");
            if (choice == null)
                return null;
            if (choice >= 1 && choice <= hotelFacilities.Count)
                return hotelFacilities[choice.Value - 1];
            Console.WriteLine("  Choose one of the listed facilities.");
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
        Console.WriteLine("\n  AVAILABLE SERVICES");
        for (int i = 0; i < facilityServices.Count; i++)
        {
            var selectedService = facilityServices[i];
            Console.WriteLine(
                $"  [{i + 1}] {selectedService.Name} | Duration: {selectedService.DurationMinutes} minutes"
            );
            if (!string.IsNullOrWhiteSpace(selectedService.Description))
                Console.WriteLine($"      {selectedService.Description}");
        }
        while (true)
        {
            int? choice = ReservationInput.ReadInt("Choose a service (q to go back): ");
            if (choice == null)
                return null;
            if (choice >= 1 && choice <= facilityServices.Count)
                return facilityServices[choice.Value - 1];
            Console.WriteLine("  Choose one of the listed services.");
        }
    }
}
