public static class GuestDetailsMenu
{
    public static UserModel? ReadGuestDetails(DateTime today)
    {
        ConsoleHelper.ShowHeader("GUEST DETAILS");
        var logic = new ReservationLogic();

        while (true)
        {
            string? firstName = ReservationInput.ReadText("First name: ");
            if (firstName == null) return null;
            string? lastName = ReservationInput.ReadText("Last name: ");
            if (lastName == null) return null;
            string? email = ReservationInput.ReadText("Email: ");
            if (email == null) return null;
            string? phone = ReservationInput.ReadText("Phone: ");
            if (phone == null) return null;
            DateTime? birthDate = ReservationInput.ReadDate("Date of birth (yyyy-MM-dd): ");
            if (birthDate == null) return null;

            var user = new UserModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                DateOfBirth = birthDate.Value
            };
            var errors = logic.ValidateGuestDetails(user, today);
            if (errors.Count == 0)
                return user;

            ShowGuestDetailsErrors(errors);
            Console.WriteLine("  Please enter your details again, or q to cancel.\n");
        }
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
