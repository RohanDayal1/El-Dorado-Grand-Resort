public class FacilityReservationLogic
{
    public List<FacilityReservationModel> GetByReservationId(int reservationId)
    {
        return FacilityReservationsAccess.GetByReservationId(reservationId);
    }

    public List<ReservationModel> GetFacilityReservations(AccountModel account, DateTime now)
    {
        return ReservationsAccess
            .GetByUserId(account.UserId)
            .Where(stay => stay.Status == "confirmed" && stay.CheckOutDate.Date > now.Date)
            .ToList();
    }

    public List<(DateTime StartTime, int RemainingCapacity)> GetAvailableStartTimes(
        AccountModel account,
        int reservationId,
        int serviceId,
        DateTime date,
        DateTime now
    )
    {
        var stay = ReservationsAccess.GetById(reservationId);
        var service = FacilitiesAccess.GetServiceById(serviceId);
        var facility = service == null ? null : FacilitiesAccess.GetById(service.FacilityId);
        if (
            stay == null
            || facility == null
            || service == null
            || FacilityReservationValidator.ValidateStay(stay, account.UserId).Count > 0
            || FacilityReservationValidator.ValidateService(facility, service).Count > 0
        )
            return new List<(DateTime StartTime, int RemainingCapacity)>();

        var bookings = FacilityReservationsAccess.GetConfirmedByFacilityAndDate(
            facility.FacilityId,
            date
        );
        return GetAvailableStartTimes(stay, facility, service, bookings, date, now);
    }

    private List<(DateTime StartTime, int RemainingCapacity)> GetAvailableStartTimes(
        ReservationModel stay,
        FacilityModel facility,
        FacilityServiceModel service,
        List<FacilityReservationModel> bookings,
        DateTime date,
        DateTime now
    )
    {
        var availableSlots = new List<(DateTime StartTime, int RemainingCapacity)>();
        for (int hour = 0; hour < 24; hour++)
        {
            DateTime startTime = date.Date.AddHours(hour);
            DateTime endTime = CalculateEndTime(startTime, service.DurationMinutes);
            if (
                FacilityReservationValidator
                    .ValidateAppointment(stay, facility, startTime, endTime, now)
                    .Count > 0
            )
                continue;
            int remainingPlaces = GetRemainingCapacity(facility, bookings, startTime, endTime);
            if (remainingPlaces > 0)
                availableSlots.Add((startTime, remainingPlaces));
        }
        return availableSlots;
    }

    public DateTime CalculateEndTime(DateTime startTime, int durationMinutes)
    {
        return startTime.AddMinutes(durationMinutes);
    }

    public List<FacilityReservationErrors> ValidatePartySize(
        int partySize, int adults, int children, int remainingCapacity)
    {
        return FacilityReservationValidator.ValidatePartySize(partySize, adults, children, remainingCapacity);
    }

    public int GetRemainingCapacity(
        FacilityModel facility,
        List<FacilityReservationModel> bookings,
        DateTime startTime,
        DateTime endTime
    )
    {
        var overlappingBookings = bookings
            .Where(booking =>
                booking.FacilityId == facility.FacilityId
                && booking.Status == "confirmed"
                && booking.StartTime < endTime
                && booking.EndTime > startTime
            )
            .ToList();

        var capacityCheckTimes = overlappingBookings
            .Where(booking => booking.StartTime > startTime)
            .Select(booking => booking.StartTime)
            .ToList();
        capacityCheckTimes.Add(startTime);

        int mostGuestsAtOnce = 0;
        foreach (var ct in capacityCheckTimes)
        {
            int guestsAtThisTime = overlappingBookings
                .Where(booking => booking.StartTime <= ct && booking.EndTime > ct)
                .Sum(booking => booking.PartySize);
            mostGuestsAtOnce = Math.Max(mostGuestsAtOnce, guestsAtThisTime);
        }
        return Math.Max(0, facility.Capacity - mostGuestsAtOnce);
    }

    public List<FacilityReservationErrors> ValidateReservation(
        AccountModel account,
        ReservationModel? reservation,
        FacilityModel? facility,
        FacilityServiceModel? service,
        DateTime startTime,
        int partySize,
        List<FacilityReservationModel> bookings,
        DateTime now
    )
    {
        var errors = FacilityReservationValidator.ValidateStay(reservation, account.UserId);
        errors.AddRange(FacilityReservationValidator.ValidateService(facility, service));
        if (errors.Count > 0 || reservation == null || facility == null || service == null)
            return errors;

        DateTime endTime = CalculateEndTime(startTime, service.DurationMinutes);
        errors.AddRange(
            FacilityReservationValidator.ValidateAppointment(
                reservation,
                facility,
                startTime,
                endTime,
                now
            )
        );
        int remaining = GetRemainingCapacity(facility, bookings, startTime, endTime);
        errors.AddRange(
            FacilityReservationValidator.ValidatePartySize(
                partySize,
                reservation.Adults,
                reservation.Children,
                remaining
            )
        );
        return errors;
    }

    public (
        FacilityReservationModel? Reservation,
        List<FacilityReservationErrors> Errors
    ) ConfirmReservation(
        AccountModel account,
        int reservationId,
        int serviceId,
        DateTime startTime,
        int partySize,
        DateTime now
    )
    {
        var stay = ReservationsAccess.GetById(reservationId);
        var service = FacilitiesAccess.GetServiceById(serviceId);
        var facility = service == null ? null : FacilitiesAccess.GetById(service.FacilityId);
        var bookings =
            facility == null
                ? new List<FacilityReservationModel>()
                : FacilityReservationsAccess.GetConfirmedByFacilityAndDate(
                    facility.FacilityId,
                    startTime.Date
                );

        var errors = ValidateReservation(
            account,
            stay,
            facility,
            service,
            startTime,
            partySize,
            bookings,
            now
        );
        if (errors.Count > 0 || stay == null || facility == null || service == null)
            return (null, errors);

        var booking = new FacilityReservationModel
        {
            AccountId = account.Id,
            ReservationId = stay.ReservationId,
            FacilityId = facility.FacilityId,
            FacilityServiceId = service.FacilityServiceId,
            ReservationDate = startTime.Date,
            StartTime = startTime,
            EndTime = CalculateEndTime(startTime, service.DurationMinutes),
            PartySize = partySize,
            Status = "confirmed",
            FacilityName = facility.Name,
            ServiceName = service.Name,
        };
        booking.FacilityReservationId = FacilityReservationsAccess.InsertReservation(booking);
        return (booking, errors);
    }

    public List<FacilityReservationModel> GetByUserId(int userId)
    {
        return FacilityReservationsAccess.GetByUserId(userId);
    }
}
