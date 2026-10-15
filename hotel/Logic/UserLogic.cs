public class UserLogic
{
    public RegisterEnum Register(string name, string lastName, string birthDate, string email, string phoneNumber)
    {
        if (!ValidateName(name))
            return RegisterEnum.InvalidName;
        if (!ValidateName(lastName))
            return RegisterEnum.InvalidLastName;
        if (!ValidateBirthdate(birthDate))
            return RegisterEnum.InvalidBirthDate;

        DateTime date = DateTime.ParseExact(birthDate, "dd-MM-yyyy", null);

        if (!ValidateEmail(email))
            return RegisterEnum.InvalidEmail;
        if (UserAccess.GetByEmail(email) != null)
            return RegisterEnum.EmailAlreadyExists;
        if (!ValidatePhoneNumber(phoneNumber))
            return RegisterEnum.InvalidPhoneNumber;
        
        UserModel user = new(name, lastName, email, phoneNumber, date);

        UserAccess.Create(user);
        return RegisterEnum.Success;
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

        if (!DateTime.TryParseExact(birthDate, "dd-MM-yyyy", null,System.Globalization.DateTimeStyles.None, out date))
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