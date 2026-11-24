public class StatisticsLogic
{
    public ResortStatisticsModel GetResortStatistics(DateTime? today = null)
    {
        DateTime reportDate = (today ?? DateTime.Today).Date;
        ResortStatisticsDataModel data = StatisticsAccess.GetResortStatisticsData();
        List<ReservationStatisticsData> confirmedReservations = data.Reservations
            .Where(reservation => HasStatus(reservation.Status, "confirmed"))
            .ToList();
        List<FacilityStatisticsData> confirmedFacilityBookings = data.Facilities
            .Where(IsConfirmedFacilityBooking)
            .ToList();
        var reviewsByRating = Enumerable.Range(1, 5)
            .Select(rating => new RatingStatistics
                { Rating = rating, ReviewCount = data.Reviews.Count(review => review.Rating == rating) })
            .ToList();

        // checks status of reservation
        int totalReservations = data.Reservations.Count;
        int cancelledReservations = data.Reservations.Count(
            reservation => HasStatus(reservation.Status, "cancelled"));
        int activeRooms = data.Rooms.Select(room => room.RoomId).Distinct().Count();
        int occupiedRooms = data.Rooms
            .Where(room =>
                HasStatus(room.ReservationStatus, "confirmed")
                && room.CheckInDate.HasValue
                && room.CheckOutDate.HasValue
                && room.CheckInDate.Value.Date <= reportDate
                && room.CheckOutDate.Value.Date > reportDate
            )
            .Select(room => room.RoomId)
            .Distinct()
            .Count();

        // calculate bookings
        decimal confirmedBookingValue = confirmedReservations.Sum(reservation => reservation.TotalCost);
        int totalReviews = data.Reviews.Count;

        List<FacilityBookingStatistics> bookingsByFacility = data.Facilities
            .GroupBy(facility => new { facility.FacilityId, facility.FacilityName })
            .Select(group => new FacilityBookingStatistics
            {
                FacilityName = group.Key.FacilityName,
                ConfirmedBookings = group.Count(IsConfirmedFacilityBooking),
            })
            .OrderBy(facility => facility.FacilityName)
            .ToList();

        var mostBookedService = confirmedFacilityBookings
            .Where(booking => booking.ServiceId.HasValue && booking.ServiceName != null)
            .GroupBy(booking => new { booking.ServiceId, booking.ServiceName })
            .Select(group => new { group.Key.ServiceName, Count = group.Count() })
            .OrderByDescending(service => service.Count)
            .ThenBy(service => service.ServiceName, StringComparer.Ordinal)
            .FirstOrDefault();
        int guestCountStayingToday = confirmedReservations
            .Where(reservation => IsStayingOn(reservation, reportDate))
            .Sum(GetGuestCount);

        return new ResortStatisticsModel
        {
            TotalReservations = totalReservations,
            ConfirmedReservations = confirmedReservations.Count,
            CancelledReservations = cancelledReservations,
            RegisteredGuests = data.Accounts.Count(account => HasStatus(account.Role, "guest")),
            ActiveRooms = activeRooms,
            OccupiedRoomsToday = occupiedRooms,
            AvailableRoomsToday = Math.Max(0, activeRooms - occupiedRooms),
            OccupancyPercentage = activeRooms == 0 ? 0 : occupiedRooms * 100m / activeRooms,
            GuestsStayingToday = guestCountStayingToday,
            ArrivingGuestsToday = confirmedReservations
                .Where(reservation => reservation.CheckInDate.Date == reportDate)
                .Sum(GetGuestCount),
            DepartingGuestsToday = confirmedReservations
                .Where(reservation => reservation.CheckOutDate.Date == reportDate)
                .Sum(GetGuestCount),
            CancellationPercentage = totalReservations == 0
                ? 0
                : cancelledReservations * 100m / totalReservations,
            ConfirmedBookingValue = confirmedBookingValue,
            AverageConfirmedBookingValue = confirmedReservations.Count == 0
                ? 0
                : confirmedBookingValue / confirmedReservations.Count,
            TotalReviews = totalReviews,
            AverageReviewRating = totalReviews == 0 ? 0 : (decimal)data.Reviews.Average(review => review.Rating),
            ConfirmedFacilityBookings = confirmedFacilityBookings.Count,
            MostBookedServiceName = mostBookedService?.ServiceName,
            MostBookedServiceCount = mostBookedService?.Count ?? 0,
            ReviewsByRating = reviewsByRating,
            BookingsByFacility = bookingsByFacility,
        };
    }

    private static bool IsConfirmedFacilityBooking(FacilityStatisticsData booking)
    {
        return booking.FacilityReservationId.HasValue
            && HasStatus(booking.FacilityReservationStatus, "confirmed")
            && HasStatus(booking.ReservationStatus, "confirmed");
    }

    private static bool HasStatus(string? actual, string expected)
    {
        return string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
    }

    private static int GetGuestCount(ReservationStatisticsData reservation)
    {
        return reservation.Adults + reservation.Children;
    }

    private static bool IsStayingOn(ReservationStatisticsData reservation, DateTime date)
    {
        return reservation.CheckInDate.Date <= date && reservation.CheckOutDate.Date > date;
    }
}
