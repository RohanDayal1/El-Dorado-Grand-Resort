public class UserValidator
{
    public UserRegisterErrors Register(string name, string lastName, string birthDate, string email, string phoneNumber)
    {
        if (!ValidateName(name))
            return UserRegisterErrors.InvalidName;
        if (!ValidateName(lastName))
            return UserRegisterErrors.InvalidLastName;
        if (!ValidateBirthdate(birthDate))
            return UserRegisterErrors.InvalidBirthDate;

        DateTime date = DateTime.ParseExact(birthDate, "yyyy-MM-dd", null);

        if (!ValidateEmail(email))
            return UserRegisterErrors.InvalidEmail;
        if (UserAccess.GetByEmail(email) != null)
            return UserRegisterErrors.EmailAlreadyExists;
        if (!ValidatePhoneNumber(phoneNumber))
            return UserRegisterErrors.InvalidPhoneNumber;
        
        UserModel user = new(name, lastName, email, phoneNumber, date);

        UserAccess.Create(user);
        return UserRegisterErrors.Success;
    }

    public bool ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
        if (name.Length < 3)
            return false;

        return true;
    }

    public bool ValidateBirthdate(string birthDate)
    {
        DateTime date;

        if (!DateTime.TryParseExact(birthDate, "yyyy-MM-dd", null,System.Globalization.DateTimeStyles.None, out date))
            return false;

        if (date > DateTime.Today)
            return false;
        
        return true;
    }

    public bool ValidateEmail(string email)
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

    public bool ValidatePhoneNumber(string phoneNumber)
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