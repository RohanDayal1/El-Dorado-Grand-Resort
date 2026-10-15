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
        var selection = GetSelectedRoomTypes(selectedRooms, availableRooms, roomTypes);
        if (selection.Errors.Count > 0)
            return selection.Errors;

        int totalCapacity = CalculateTotalCapacity(selection.RoomTypes);
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
        decimal totalCost = 0m;

        var selection = GetSelectedRoomTypes(selectedRooms, availableRooms, roomTypes);
        if (selection.Errors.Count > 0)
            return (0m, selection.Errors);

        foreach (var type in selection.RoomTypes)
        {
            totalCost += CalculateRoomCost(type, checkInDate, checkOutDate);
        }
        return (totalCost, selection.Errors);
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
        var selectedTypes = new List<RoomTypeModel>();
        if (errors.Count > 0)
            return (selectedTypes, errors);

        foreach (var selected in selectedRooms)
        {
            var room = availableRooms.First(room => room.RoomId == selected.RoomId);
            var type = roomTypes.First(type => type.Room_Type_Id == room.Room_Type_Id);
            selectedTypes.Add(type);
        }
        return (selectedTypes, errors);
    }

    public (ReservationModel? Reservation, List<ReservationConfirmationErrors> Errors) ConfirmGuestReservation(
        UserModel user, DateTime checkInDate, DateTime checkOutDate,
        int adults, int children, List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms, DateTime today)
    {
        return ConfirmReservation(user, checkInDate, checkOutDate, adults, children,
            selectedRooms, availableRooms, today, false);
    }

    public (ReservationModel? Reservation, List<ReservationConfirmationErrors> Errors) ConfirmAccountReservation(
        AccountModel account, DateTime checkInDate, DateTime checkOutDate,
        int adults, int children, List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms, DateTime today)
    {
        UserModel? user;
        try
        {
            user = new AccountsLogic().GetGuestDetails(account);
        }
        catch (Exception)
        {
            user = null;
        }
        if (user == null)
            return (null, new List<ReservationConfirmationErrors> { ReservationConfirmationErrors.AccountUnavailable });
        return ConfirmReservation(user, checkInDate, checkOutDate, adults, children,
            selectedRooms, availableRooms, today, true);
    }

    private (ReservationModel? Reservation, List<ReservationConfirmationErrors> Errors) ConfirmReservation(
        UserModel user, DateTime checkInDate, DateTime checkOutDate,
        int adults, int children, List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms, DateTime today, bool useExistingUser)
    {
        var errors = new List<ReservationConfirmationErrors>();
        if (ValidateDates(checkInDate, checkOutDate, today).Count > 0)
            errors.Add(ReservationConfirmationErrors.InvalidDates);
        if (ValidateGuestCounts(adults, children).Count > 0)
            errors.Add(ReservationConfirmationErrors.InvalidGuestCounts);
        if (ValidateGuestDetails(user, today).Count > 0)
            errors.Add(ReservationConfirmationErrors.InvalidGuestDetails);
        if (errors.Count > 0)
            return (null, errors);

        List<RoomTypeModel> roomTypes;
        try
        {
            roomTypes = RoomsAccess.GetRoomTypes();
        }
        catch (Exception)
        {
            errors.Add(ReservationConfirmationErrors.RoomDataUnavailable);
            return (null, errors);
        }

        if (ValidateRoomSelection(adults, children, selectedRooms, availableRooms, roomTypes).Count > 0)
        {
            errors.Add(ReservationConfirmationErrors.InvalidRoomSelection);
            return (null, errors);
        }
        var cost = CalculateTotalCost(checkInDate, checkOutDate, selectedRooms, availableRooms, roomTypes);
        if (cost.Errors.Count > 0)
        {
            errors.Add(ReservationConfirmationErrors.InvalidRoomSelection);
            return (null, errors);
        }

        string code;
        try
        {
            code = GenerateReservationCode();
        }
        catch (Exception)
        {
            errors.Add(ReservationConfirmationErrors.ReservationCodeGenerationFailed);
            return (null, errors);
        }

        var reservation = new ReservationModel
        {
            ReservationCode = code,
            CheckInDate = checkInDate.Date,
            CheckOutDate = checkOutDate.Date,
            Adults = adults,
            Children = children,
            TotalCost = cost.TotalCost,
            Status = "confirmed"
        };
        try
        {
            reservation.ReservationId = useExistingUser
                ? ReservationsAccess.SaveReservationForUser(user.UserId, reservation, selectedRooms)
                : ReservationsAccess.SaveGuestReservation(user, reservation, selectedRooms);
        }
        catch (Exception)
        {
            errors.Add(ReservationConfirmationErrors.ReservationSaveFailed);
            return (null, errors);
        }
        return (reservation, errors);
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

