public class StatisticsLogic
{
    public ResortStatisticsModel GetResortStatistics(DateTime? today = null)
    {
        DateTime reportDate = (today ?? DateTime.Today).Date;
        var data = StatisticsAccess.GetResortStatisticsData();
        var reservationsById = data.Reservations.ToDictionary(item => item.ReservationId);
        var confirmedReservations = data.Reservations
            .Where(item => HasStatus(item.Status, "confirmed"))
            .ToList();
        var activeRoomIds = data.Rooms
            .Where(room => HasStatus(room.Status, "active"))
            .Select(room => room.RoomId)
            .ToHashSet();
        var occupiedRoomIds = data.ReservationRooms
            .Where(assignment =>
                activeRoomIds.Contains(assignment.RoomId)
                && reservationsById.TryGetValue(assignment.ReservationId, out var reservation)
                && HasStatus(reservation.Status, "confirmed")
                && IsStayingOn(reservation, reportDate)
            )
            .Select(assignment => assignment.RoomId)
            .ToHashSet();
        var confirmedFacilityBookings = data.FacilityReservations
            .Where(booking =>
                HasStatus(booking.Status, "confirmed")
                && reservationsById.TryGetValue(booking.ReservationId, out var reservation)
                && HasStatus(reservation.Status, "confirmed")
            )
            .ToList();

        int totalReservations = data.Reservations.Count;
        int cancelledReservations = data.Reservations.Count(
            item => HasStatus(item.Status, "cancelled"));
        int totalReviews = data.Reviews.Count;
        decimal confirmedBookingValue = confirmedReservations.Sum(item => item.TotalCost);
        var serviceNames = data.FacilityServices.ToDictionary(
            service => service.FacilityServiceId,
            service => service.Name
        );
        var mostBookedService = confirmedFacilityBookings
            .GroupBy(booking => booking.FacilityServiceId)
            .Select(group => new
            {
                Name = serviceNames[group.Key],
                Count = group.Count(),
            })
            .OrderByDescending(service => service.Count)
            .ThenBy(service => service.Name, StringComparer.Ordinal)
            .FirstOrDefault();

        return new ResortStatisticsModel
        {
            TotalReservations = totalReservations,
            ConfirmedReservations = confirmedReservations.Count,
            CancelledReservations = cancelledReservations,
            RegisteredGuests = data.Accounts.Count(account => HasStatus(account.Role, "guest")),
            ActiveRooms = activeRoomIds.Count,
            OccupiedRoomsToday = occupiedRoomIds.Count,
            AvailableRoomsToday = Math.Max(0, activeRoomIds.Count - occupiedRoomIds.Count),
            OccupancyPercentage = activeRoomIds.Count == 0
                ? 0
                : occupiedRoomIds.Count * 100m / activeRoomIds.Count,
            GuestsStayingToday = confirmedReservations
                .Where(reservation => IsStayingOn(reservation, reportDate))
                .Sum(GetGuestCount),
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
            AverageReviewRating = totalReviews == 0
                ? 0
                : (decimal)data.Reviews.Average(review => review.Rating),
            ConfirmedFacilityBookings = confirmedFacilityBookings.Count,
            MostBookedServiceName = mostBookedService?.Name,
            MostBookedServiceCount = mostBookedService?.Count ?? 0,
            ReviewsByRating = Enumerable.Range(1, 5)
                .Select(rating => new RatingStatistics
                {
                    Rating = rating,
                    ReviewCount = data.Reviews.Count(review => review.Rating == rating),
                })
                .ToList(),
            BookingsByFacility = data.Facilities
                .Select(facility => new FacilityBookingStatistics
                {
                    FacilityName = facility.Name,
                    ConfirmedBookings = confirmedFacilityBookings.Count(
                        booking => booking.FacilityId == facility.FacilityId
                    ),
                })
                .OrderBy(facility => facility.FacilityName)
                .ToList(),
        };
    }

    private static bool HasStatus(string? actual, string expected)
    {
        return string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
    }

    private static int GetGuestCount(ReservationModel reservation)
    {
        return reservation.Adults + reservation.Children;
    }

    private static bool IsStayingOn(ReservationModel reservation, DateTime date)
    {
        return reservation.CheckInDate.Date <= date && reservation.CheckOutDate.Date > date;
    }
}
