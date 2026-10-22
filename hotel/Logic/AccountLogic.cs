public class AccountLogic
{
    public static AccountModel? CurrentAccount { get; private set; }

    public static RegisterEnum Register(int userId, string password, string repeatedpassword, string role)
    {
        if (!ValidateUser(userId))
            return RegisterEnum.InvalidUser;
        if (!ValidatePassword(password))
            return RegisterEnum.InvalidPassword;
        if (password != repeatedpassword)
            return RegisterEnum.PasswordMismatch;
        if (string.IsNullOrWhiteSpace(role))
            return RegisterEnum.InvalidRole;
        
        AccountModel account = new(userId, password, role);
        
        AccountAccess.Create(account);

        return RegisterEnum.Success;
    }

    public static bool ValidateUser(int userid)
    {
       UserModel user = UserAccess.GetById(userid);
       if (user == null) return false;

       return true;
    }
    public static bool ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (password.Length < 8 || password.Length > 20) return false;

        bool hasUpper = false;
        bool hasDigit = false;
        bool hasSpecial = false;

        foreach (char character in password)
        {
            if (char.IsUpper(character)) hasUpper = true;
            if (char.IsDigit(character)) hasDigit = true;
            if (!char.IsLetterOrDigit(character)) hasSpecial = true;
        }

        if (!hasUpper || !hasDigit || !hasSpecial)
            return false;

        return true;

    }


}