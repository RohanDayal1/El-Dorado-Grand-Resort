public class ResortStatisticsModel
{
    public int TotalReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int CancelledReservations { get; set; }
    public int RegisteredGuests { get; set; }
    public int ActiveRooms { get; set; }
    public int OccupiedRoomsToday { get; set; }
    public int AvailableRoomsToday { get; set; }
    public decimal OccupancyPercentage { get; set; }
    public int GuestsStayingToday { get; set; }
    public int ArrivingGuestsToday { get; set; }
    public int DepartingGuestsToday { get; set; }
    public decimal CancellationPercentage { get; set; }
    public decimal ConfirmedBookingValue { get; set; }
    public decimal AverageConfirmedBookingValue { get; set; }
    public int TotalReviews { get; set; }
    public decimal? AverageReviewRating { get; set; }
    public int ConfirmedFacilityBookings { get; set; }
    public string? MostBookedServiceName { get; set; }
    public int MostBookedServiceCount { get; set; }
    public List<RatingStatistics> ReviewsByRating { get; set; } = new();
    public List<FacilityBookingStatistics> BookingsByFacility { get; set; } = new();
}

public class RatingStatistics
{
    public int Rating { get; set; }
    public int ReviewCount { get; set; }
}

public class FacilityBookingStatistics
{
    public string FacilityName { get; set; } = string.Empty;
    public int ConfirmedBookings { get; set; }
}
