public static class UserValidator
{
    public static List<string> Register(string name, string lastName, string birthDate, string email, string phoneNumber)
    {
        List<string> errors = new();

        if (!ValidateName(name))
            errors.Add("\n  Name must be 3 characters or longer.");
        if (!ValidateName(lastName))
            errors.Add("\n  Last name must be 3 characters or longer.");
        if (!ValidateBirthdate(birthDate))
            errors.Add("\n  Invalid Birthdate, format must be in yyyy-MM-dd.");

        if (!ValidateEmail(email))
            errors.Add("\n  Invalid E-Mail.");
        if (UserAccess.GetByEmail(email) != null)
            errors.Add("\n  E-Mail already in use.");
        if (!ValidatePhoneNumber(phoneNumber))
            errors.Add("\n  Invalid Phone number.");
        
        if (errors.Count == 0)
        {
            DateTime date = DateTime.ParseExact(birthDate, "yyyy-MM-dd", null);
            UserModel user = new(name, lastName, email, phoneNumber, date);
            UserAccess.Create(user);
        }

        return errors;
    }

    public static bool ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        if (name.Length < 3)
            return false;

        return true;
    }

    public static bool ValidateBirthdate(string birthDate)
    {
        DateTime date;

        if (!DateTime.TryParseExact(birthDate, "yyyy-MM-dd", null,System.Globalization.DateTimeStyles.None, out date))
            return false;

        if (date > DateTime.Today)
            return false;
        
        return true;
    }

    public static bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        if (!email.Contains("@") || !email.Contains(".")) return false;

        int charPosX = email.IndexOf("@");
        int charPosY = email.LastIndexOf(".");

        if (charPosY < charPosX) return false;
        if (charPosX <= 0) return false;
        if (charPosY == email.Length - 1) return false;
        if (charPosY == charPosX + 1) return false;

        return true;
    }

    public static bool ValidatePhoneNumber(string phoneNumber)
    {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            if (phoneNumber.Length != 10)
                return false;

            if (!phoneNumber.StartsWith("06"))
                return false;

            return phoneNumber.All(char.IsDigit);
    }
   
}