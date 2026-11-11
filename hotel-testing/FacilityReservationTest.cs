namespace HotelTesting;

[TestClass]
public class FacilityReservationTest
{
    private readonly FacilityReservationLogic logic = new FacilityReservationLogic();
    private readonly AccountModel account = new AccountModel { Id = 1, UserId = 1 };
    private readonly ReservationModel stay = new ReservationModel
    {
        ReservationId = 1,
        ReservationCode = "A1",
        UserId = 1,
        CheckInDate = new DateTime(2027, 3, 10),
        CheckOutDate = new DateTime(2027, 3, 13),
        Adults = 2,
        Children = 2,
        Status = "confirmed",
    };
    private readonly FacilityModel spa = new FacilityModel
    {
        FacilityId = 2,
        Name = "Spa",
        Capacity = 20,
        OpeningHours = new DateTime(2027, 3, 11, 9, 0, 0),
        ClosingHours = new DateTime(2027, 3, 11, 20, 0, 0),
    };
    private readonly FacilityServiceModel service = new FacilityServiceModel
    {
        FacilityServiceId = 1,
        FacilityId = 2,
        Name = "Massage",
        DurationMinutes = 60,
    };
    private readonly DateTime now = new DateTime(2027, 3, 11, 8, 0, 0);

    // one person, starting exactly when the spa opens.
    [TestMethod]
    public void H3aValidateReservation_OnePersonAtOpening_NoError()
    {
        DateTime startTime = new DateTime(2027, 3, 11, 9, 0, 0);

        var errors = logic.ValidateReservation(
            account, stay, spa, service, startTime, 1,
            new List<FacilityReservationModel>(), now);

        Assert.AreEqual(0, errors.Count);
    }

    // all four hotel guests, ending exactly when the spa closes.
    [TestMethod]
    public void H3bValidateReservation_FourPeopleAtClosing_NoError()
    {
        DateTime startTime = new DateTime(2027, 3, 11, 19, 0, 0);

        var errors = logic.ValidateReservation(
            account, stay, spa, service, startTime, 4,
            new List<FacilityReservationModel>(), now);

        Assert.AreEqual(0, errors.Count);
    }

    //a cancelled hotel reservation cannot be used.
    [TestMethod]
    public void S2aValidateReservation_CancelledStay_ReservationNotConfirmedError()
    {
        stay.Status = "cancelled";
        DateTime startTime = new DateTime(2027, 3, 11, 10, 0, 0);

        var errors = logic.ValidateReservation(
            account, stay, spa, service, startTime, 2,
            new List<FacilityReservationModel>(), now);

        CollectionAssert.AreEqual(
            new[] { FacilityReservationErrors.ReservationNotConfirmed }, errors);
    }

    //reject 10:00 when it is already 11:00, then accept 14:00.
    [TestMethod]
    public void S3aValidateReservation_PastStartThenFutureStart_AppointmentInPastError()
    {
        DateTime currentTime = new DateTime(2027, 3, 11, 11, 0, 0);
        DateTime pastStart = new DateTime(2027, 3, 11, 10, 0, 0);
        DateTime futureStart = new DateTime(2027, 3, 11, 14, 0, 0);

        var pastErrors = logic.ValidateReservation(
            account, stay, spa, service, pastStart, 2,
            new List<FacilityReservationModel>(), currentTime);
        var futureErrors = logic.ValidateReservation(
            account, stay, spa, service, futureStart, 2,
            new List<FacilityReservationModel>(), currentTime);

        CollectionAssert.AreEqual(
            new[] { FacilityReservationErrors.AppointmentInPast }, pastErrors);
        Assert.AreEqual(0, futureErrors.Count);
    }

    //the spa opens at 09:00.
    [TestMethod]
    public void S4aValidateReservation_BeforeOpeningHours_OutsideOpeningHoursError()
    {
        DateTime startTime = new DateTime(2027, 3, 11, 8, 0, 0);

        var errors = logic.ValidateReservation(
            account, stay, spa, service, startTime, 2,
            new List<FacilityReservationModel>(), now);

        CollectionAssert.AreEqual(
            new[] { FacilityReservationErrors.OutsideOpeningHours }, errors);
    }

    //treatment at 19:00 ends after closing.
    [TestMethod]
    public void S4bValidateReservation_EndAfterClosing_OutsideOpeningHoursError()
    {
        service.DurationMinutes = 90;
        DateTime startTime = new DateTime(2027, 3, 11, 19, 0, 0);

        var errors = logic.ValidateReservation(
            account, stay, spa, service, startTime, 2,
            new List<FacilityReservationModel>(), now);

        CollectionAssert.AreEqual(
            new[] { FacilityReservationErrors.OutsideOpeningHours }, errors);
    }

    //reject zero people, then accept one person.
    [TestMethod]
    public void S5aValidatePartySize_ZeroThenOne_PartySizeBelowOneError()
    {
        var invalidErrors = logic.ValidatePartySize(0, stay.Adults, stay.Children, 20);
        var validErrors = logic.ValidatePartySize(1, stay.Adults, stay.Children, 20);

        CollectionAssert.AreEqual(
            new[] { FacilityReservationErrors.PartySizeBelowOne }, invalidErrors);
        Assert.AreEqual(0, validErrors.Count);
    }

    //two adults and two children allow at most four people.
    [TestMethod]
    public void S5bValidatePartySize_ToomanyPeople_PartySizeExceedsGuestCountError()
    {
        var invalidErrors = logic.ValidatePartySize(5, stay.Adults, stay.Children, 20);
        var validErrors = logic.ValidatePartySize(4, stay.Adults, stay.Children, 20);

        CollectionAssert.AreEqual(
            new[] { FacilityReservationErrors.PartySizeExceedsGuestCount }, invalidErrors);
        Assert.AreEqual(0, validErrors.Count);
    }

    //two people do not fit, but one does.
    [TestMethod]
    public void S7aValidateReservation_OnePlaceLeft_InsufficientCapacityError()
    {
        DateTime startTime = new DateTime(2027, 3, 11, 10, 0, 0);
        DateTime endTime = new DateTime(2027, 3, 11, 11, 0, 0);
        var bookings = new List<FacilityReservationModel>
        {
            new FacilityReservationModel
            {
                FacilityId = spa.FacilityId,
                StartTime = startTime,
                EndTime = endTime,
                PartySize = 19,
                Status = "confirmed",
            },
        };

        int remainingCapacity = logic.GetRemainingCapacity(spa, bookings, startTime, endTime);
        var invalidErrors = logic.ValidateReservation(
            account, stay, spa, service, startTime, 2, bookings, now);
        var validErrors = logic.ValidateReservation(
            account, stay, spa, service, startTime, 1, bookings, now);

        Assert.AreEqual(1, remainingCapacity);
        CollectionAssert.AreEqual(
            new[] { FacilityReservationErrors.InsufficientCapacity }, invalidErrors);
        Assert.AreEqual(0, validErrors.Count);
    }
}
