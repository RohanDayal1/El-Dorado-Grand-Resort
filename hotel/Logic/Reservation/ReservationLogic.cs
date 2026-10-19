public class ReservationLogic
{
    public IEnumerable<ReservationModel> GetByUserId(int userId)
    {
        return ReservationsAccess.GetByUserId(userId);
    }

    public List<ReservationDateErrors> ValidateDates(
        DateTime checkInDate,
        DateTime checkOutDate,
        DateTime today
    )
    {
        return ReservationValidator.ValidateDate(checkInDate, checkOutDate, today);
    }

    public List<GuestCountErrors> ValidateGuestCounts(int adults, int children)
    {
        return GuestValidator.ValidateGuestCount(adults, children);
    }

    public List<GuestDetailsErrors> ValidateGuestDetails(UserModel user, DateTime today)
    {
        return GuestValidator.ValidateGuestDetails(user, today);
    }

    public List<RoomQuantityErrors> ValidateRoomQuantity(int quantity, int availableQuantity)
    {
        return ReservationValidator.ValidateRoomQuantity(quantity, availableQuantity);
    }

    public List<RoomSelectionErrors> ValidateRoomSelection(
        int adults,
        int children,
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes
    )
    {
        var (selectedTypes, errors) = GetSelectedRoomTypes(
            selectedRooms,
            availableRooms,
            roomTypes
        );
        if (errors.Count > 0)
            return errors;

        int totalCapacity = CalculateTotalCapacity(selectedTypes);
        return ReservationValidator.ValidateRoomSelection(
            adults,
            children,
            selectedRooms.Count,
            totalCapacity
        );
    }

    public int CalculateTotalCapacity(List<RoomTypeModel> selectedRoomTypes)
    {
        return selectedRoomTypes.Sum(type => type.Capacity);
    }

    public decimal CalculateRoomCost(
        RoomTypeModel roomType,
        DateTime checkInDate,
        DateTime checkOutDate
    )
    {
        int nights = (checkOutDate.Date - checkInDate.Date).Days;
        return roomType.PricePerNight * nights;
    }

    public (decimal TotalCost, List<RoomSelectionErrors> Errors) CalculateTotalCost(
        DateTime checkInDate,
        DateTime checkOutDate,
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes
    )
    {
        var (selectedTypes, errors) = GetSelectedRoomTypes(
            selectedRooms,
            availableRooms,
            roomTypes
        );
        if (errors.Count > 0)
            return (0m, errors);

        decimal totalCost = selectedTypes.Sum(type =>
            CalculateRoomCost(type, checkInDate, checkOutDate)
        );
        return (totalCost, errors);
    }

    private (List<RoomTypeModel> RoomTypes, List<RoomSelectionErrors> Errors) GetSelectedRoomTypes(
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes
    )
    {
        var errors = ReservationValidator.ValidateSelectedRoomData(
            selectedRooms,
            availableRooms,
            roomTypes
        );
        if (errors.Count > 0)
            return (new List<RoomTypeModel>(), errors);

        var selectedTypes = selectedRooms
            .Select(selected => availableRooms.First(room => room.RoomId == selected.RoomId))
            .Select(room => roomTypes.First(type => type.Room_Type_Id == room.Room_Type_Id))
            .ToList();

        return (selectedTypes, errors);
    }

    public ReservationModel ConfirmGuestReservation(
        UserModel user,
        DateTime checkInDate,
        DateTime checkOutDate,
        int adults,
        int children,
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        DateTime today
    )
    {
        return ConfirmReservation(
            user,
            checkInDate,
            checkOutDate,
            adults,
            children,
            selectedRooms,
            availableRooms,
            today,
            false
        );
    }

    public ReservationModel ConfirmAccountReservation(
        AccountModel account,
        DateTime checkInDate,
        DateTime checkOutDate,
        int adults,
        int children,
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        DateTime today
    )
    {
        UserModel user =
            new AccountsLogic().GetGuestDetails(account)
            ?? throw new InvalidOperationException("The account has no guest details.");

        return ConfirmReservation(
            user,
            checkInDate,
            checkOutDate,
            adults,
            children,
            selectedRooms,
            availableRooms,
            today,
            true
        );
    }

    private ReservationModel ConfirmReservation(
        UserModel user,
        DateTime checkInDate,
        DateTime checkOutDate,
        int adults,
        int children,
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        DateTime today,
        bool useExistingUser
    )
    {
        List<RoomTypeModel> roomTypes = RoomsAccess.GetRoomTypes();

        bool isValid =
            ValidateDates(checkInDate, checkOutDate, today).Count == 0
            && ValidateGuestCounts(adults, children).Count == 0
            && ValidateGuestDetails(user, today).Count == 0
            && ValidateRoomSelection(
                adults,
                children,
                selectedRooms,
                availableRooms,
                roomTypes
            ).Count == 0;

        if (!isValid)
            throw new InvalidOperationException("The reservation data is not valid.");

        var cost = CalculateTotalCost(
            checkInDate,
            checkOutDate,
            selectedRooms,
            availableRooms,
            roomTypes
        );

        var reservation = new ReservationModel
        {
            ReservationCode = GenerateReservationCode(),
            CheckInDate = checkInDate.Date,
            CheckOutDate = checkOutDate.Date,
            TotalCost = cost.TotalCost,
            Status = "confirmed",
        };

        reservation.ReservationId = useExistingUser
            ? ReservationsAccess.SaveReservationForUser(user.UserId, reservation, selectedRooms)
            : ReservationsAccess.SaveGuestReservation(user, reservation, selectedRooms);

        return reservation;
    }

    private string GenerateReservationCode()
    {
        const string characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        string code;

        do
        {
            code = "";
            for (int i = 0; i < 6; i++)
                code += characters[Random.Shared.Next(characters.Length)];
        } while (ReservationsAccess.ReservationCodeExists(code));

        return code;
    }
}
