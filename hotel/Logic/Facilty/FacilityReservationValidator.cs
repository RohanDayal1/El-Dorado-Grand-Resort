public static class FacilityReservationValidator
{
    public static List<FacilityReservationErrors> ValidateStay(
        ReservationModel? reservation,
        int userId
    )
    {
        var errors = new List<FacilityReservationErrors>();
        if (reservation == null)
        {
            errors.Add(FacilityReservationErrors.ReservationNotFound);
            return errors;
        }
        if (reservation.Status != "confirmed")
            errors.Add(FacilityReservationErrors.ReservationNotConfirmed);
        return errors;
    }

    public static List<FacilityReservationErrors> ValidateService(
        FacilityModel? facility,
        FacilityServiceModel? service
    )
    {
        var errors = new List<FacilityReservationErrors>();
        if (facility == null)
            errors.Add(FacilityReservationErrors.FacilityNotFound);
        if (service == null)
            errors.Add(FacilityReservationErrors.ServiceNotFound);
        if (facility == null || service == null)
            return errors;

        if (service.FacilityId != facility.FacilityId)
            errors.Add(FacilityReservationErrors.ServiceNotForFacility);
        if (service.DurationMinutes <= 0)
            errors.Add(FacilityReservationErrors.InvalidServiceDuration);
        return errors;
    }

    public static List<FacilityReservationErrors> ValidateAppointment(
        ReservationModel reservation,
        FacilityModel facility,
        DateTime startTime,
        DateTime endTime,
        DateTime now
    )
    {
        var errors = new List<FacilityReservationErrors>();
        if (!FitsWithinStay(startTime, reservation.CheckInDate, reservation.CheckOutDate))
            errors.Add(FacilityReservationErrors.OutsideStay);
        if (startTime < now)
            errors.Add(FacilityReservationErrors.AppointmentInPast);
        if (startTime != startTime.Date.AddHours(startTime.Hour))
            errors.Add(FacilityReservationErrors.StartTimeNotOnHour);

        if (facility.OpeningHours == null || facility.ClosingHours == null)
        {
            errors.Add(FacilityReservationErrors.OpeningHoursUnavailable);
            return errors;
        }

        DateTime opening = startTime
            .Date.AddHours(facility.OpeningHours.Value.Hour)
            .AddMinutes(facility.OpeningHours.Value.Minute);
        DateTime closing = startTime
            .Date.AddHours(facility.ClosingHours.Value.Hour)
            .AddMinutes(facility.ClosingHours.Value.Minute);
        if (closing <= opening || startTime < opening || endTime > closing)
            errors.Add(FacilityReservationErrors.OutsideOpeningHours);
        return errors;
    }

    public static List<FacilityReservationErrors> ValidatePartySize(
        int partySize,
        int adults,
        int children,
        int remainingCapacity
    )
    {
        var errors = new List<FacilityReservationErrors>();
        if (partySize < 1)
            errors.Add(FacilityReservationErrors.PartySizeBelowOne);
        if (partySize > adults + children)
            errors.Add(FacilityReservationErrors.PartySizeExceedsGuestCount);
        if (remainingCapacity < partySize)
            errors.Add(FacilityReservationErrors.InsufficientCapacity);
        return errors;
    }

    public static bool FitsWithinStay(
        DateTime appointmentDate,
        DateTime checkInDate,
        DateTime checkOutDate
    )
    {
        return appointmentDate.Date >= checkInDate.Date && appointmentDate.Date < checkOutDate.Date;
    }
}
