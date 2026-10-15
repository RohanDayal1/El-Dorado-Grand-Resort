public static class GuestValidator
{
    public static List<GuestDetailsErrors> ValidateGuestDetails(UserModel user, DateTime today)
    {
        var errors = new List<GuestDetailsErrors>();

        if (string.IsNullOrWhiteSpace(user.FirstName))
            errors.Add(GuestDetailsErrors.FirstNameRequired);

        if (string.IsNullOrWhiteSpace(user.LastName))
            errors.Add(GuestDetailsErrors.LastNameRequired);

        if (!System.Net.Mail.MailAddress.TryCreate(user.Email, out var email) || email.Address != user.Email)
            errors.Add(GuestDetailsErrors.InvalidEmail);

        if (string.IsNullOrWhiteSpace(user.Phone))
            errors.Add(GuestDetailsErrors.PhoneRequired);

        if (!user.DateOfBirth.HasValue)
            errors.Add(GuestDetailsErrors.DateOfBirthRequired);

        if (user.DateOfBirth.HasValue && user.DateOfBirth.Value.Date > today.Date)
            errors.Add(GuestDetailsErrors.DateOfBirthInFuture);

        if (user.DateOfBirth.HasValue && user.DateOfBirth.Value.Date <= today.Date &&
            user.DateOfBirth.Value.Date > today.Date.AddYears(-18))
            errors.Add(GuestDetailsErrors.BookerUnder18);

        return errors;
    }

    public static List<GuestCountErrors> ValidateGuestCount(int adults, int children)
    {
        var errors = new List<GuestCountErrors>();

        if (adults < 1)
            errors.Add(GuestCountErrors.NoAdults);

        if (children < 0)
            errors.Add(GuestCountErrors.NegativeChildren);

        return errors;
    }
}
