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

public class ResortStatisticsDataModel
{
    public List<ReservationStatisticsData> Reservations { get; set; } = new();
    public List<AccountStatisticsData> Accounts { get; set; } = new();
    public List<RoomStatisticsData> Rooms { get; set; } = new();
    public List<ReviewStatisticsData> Reviews { get; set; } = new();
    public List<FacilityStatisticsData> Facilities { get; set; } = new();
}

public class ReservationStatisticsData
{
    public string Status { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal TotalCost { get; set; }
}

public class AccountStatisticsData
{
    public string Role { get; set; } = string.Empty;
}

public class RoomStatisticsData
{
    public int RoomId { get; set; }
    public string? ReservationStatus { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
}

public class ReviewStatisticsData
{
    public int Rating { get; set; }
}

public class FacilityStatisticsData
{
    public int FacilityId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public int? FacilityReservationId { get; set; }
    public string? FacilityReservationStatus { get; set; }
    public string? ReservationStatus { get; set; }
    public int? ServiceId { get; set; }
    public string? ServiceName { get; set; }
}
