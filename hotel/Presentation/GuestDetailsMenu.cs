public static class GuestDetailsMenu
{
    public static UserModel? ReadGuestDetails(DateTime today)
    {
        ConsoleHelper.ShowHeader("GUEST DETAILS");
        var user = new UserModel();

        do
        {
            string? firstName = ReservationInput.ReadText("First name: ");
            if (firstName == null) return null;
            user.FirstName = firstName;
        } while (ShowFieldErrors(user, today, GuestDetailsErrors.FirstNameRequired));

        do
        {
            string? lastName = ReservationInput.ReadText("Last name: ");
            if (lastName == null) return null;
            user.LastName = lastName;
        } while (ShowFieldErrors(user, today, GuestDetailsErrors.LastNameRequired));

        do
        {
            string? email = ReservationInput.ReadText("Email: ");
            if (email == null) return null;
            user.Email = email;
        } while (ShowFieldErrors(user, today, GuestDetailsErrors.InvalidEmail));

        do
        {
            string? phone = ReservationInput.ReadText("Phone: ");
            if (phone == null) return null;
            user.Phone = phone;
        } while (ShowFieldErrors(user, today, GuestDetailsErrors.PhoneRequired));

        do
        {
            DateTime? birthDate = ReservationInput.ReadDate("Date of birth (yyyy-MM-dd): ");
            if (birthDate == null) return null;
            user.DateOfBirth = birthDate.Value;
        } while (ShowFieldErrors(user, today, GuestDetailsErrors.DateOfBirthRequired,
            GuestDetailsErrors.DateOfBirthInFuture, GuestDetailsErrors.BookerUnder18));

        return user;
    }

    private static bool ShowFieldErrors(UserModel user, DateTime today,
        params GuestDetailsErrors[] fieldErrors)
    {
        var errors = new ReservationLogic().ValidateGuestDetails(user, today);
        var currentFieldErrors = new List<GuestDetailsErrors>();
        foreach (var error in errors)
        {
            if (fieldErrors.Contains(error))
                currentFieldErrors.Add(error);
        }

        ShowGuestDetailsErrors(currentFieldErrors);
        return currentFieldErrors.Count > 0;
    }

    private static void ShowGuestDetailsErrors(List<GuestDetailsErrors> errors)
    {
        foreach (var error in errors)
        {
            string message = error switch
            {
                GuestDetailsErrors.FirstNameRequired => "Enter your first name.",
                GuestDetailsErrors.LastNameRequired => "Enter your last name.",
                GuestDetailsErrors.InvalidEmail => "Enter a valid email address.",
                GuestDetailsErrors.PhoneRequired => "Enter your phone number.",
                GuestDetailsErrors.DateOfBirthRequired => "Enter your date of birth.",
                GuestDetailsErrors.DateOfBirthInFuture => "Your date of birth cannot be in the future.",
                GuestDetailsErrors.BookerUnder18 => "The person making the reservation must be at least 18.",
                _ => "Please check your guest details."
            };
            Console.WriteLine($"  {message}");
        }
    }
}
