public enum FacilityReservationErrors
{
    ReservationNotFound,
    ReservationNotConfirmed,
    FacilityNotFound,
    ServiceNotFound,
    ServiceNotForFacility,
    InvalidServiceDuration,
    OutsideStay,
    AppointmentInPast,
    StartTimeNotOnHour,
    OpeningHoursUnavailable,
    OutsideOpeningHours,
    PartySizeBelowOne,
    PartySizeExceedsGuestCount,
    InsufficientCapacity
}
