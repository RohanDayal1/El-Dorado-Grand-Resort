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
        var errors = ValidateSelectedRoomData(
            selectedRooms,
            availableRooms,
            roomTypes
        );
        if (errors.Count > 0)
            return errors;

        var selectedTypes = GetSelectedRoomTypes(selectedRooms, availableRooms, roomTypes);
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

    public decimal CalculateTotalCost(
        DateTime checkInDate,
        DateTime checkOutDate,
        List<RoomTypeModel> selectedRoomTypes
    )
    {
        decimal totalCost = 0m;
        foreach (var type in selectedRoomTypes)
        {
            totalCost += CalculateRoomCost(type, checkInDate, checkOutDate);
        }
        return totalCost;
    }

    public List<RoomSelectionErrors> ValidateSelectedRoomData(
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes
    )
    {
        return ReservationValidator.ValidateSelectedRoomData(
            selectedRooms,
            availableRooms,
            roomTypes
        );
    }

    public List<RoomTypeModel> GetSelectedRoomTypes(
        List<ReservationRoomModel> selectedRooms,
        List<RoomModel> availableRooms,
        List<RoomTypeModel> roomTypes)
    {
        var selectedTypes = new List<RoomTypeModel>();
        foreach (var selected in selectedRooms)
        {
            var room = availableRooms.First(room => room.RoomId == selected.RoomId);
            var type = roomTypes.First(type => type.Room_Type_Id == room.Room_Type_Id);
            selectedTypes.Add(type);
        }
        return selectedTypes;
    }

    public ReservationModel ConfirmGuestReservation(
        UserModel user,
        DateTime checkInDate,
        DateTime checkOutDate,
        int adults,
        int children,
        List<ReservationRoomModel> selectedRooms,
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
        DateTime today,
        bool useExistingUser
    )
    {
        List<RoomModel> availableRooms = RoomsAccess.GetAvailableRooms(checkInDate, checkOutDate);
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

        var selectedTypes = GetSelectedRoomTypes(selectedRooms, availableRooms, roomTypes);
        decimal cost = CalculateTotalCost(
            checkInDate,
            checkOutDate,
            selectedTypes
        );

        var reservation = new ReservationModel
        {
            ReservationCode = GenerateReservationCode(),
            CheckInDate = checkInDate.Date,
            CheckOutDate = checkOutDate.Date,
            Adults = adults,
            Children = children,
            TotalCost = cost,
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
