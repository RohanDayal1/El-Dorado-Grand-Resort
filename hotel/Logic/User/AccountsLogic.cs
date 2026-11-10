//This class is not static so later on we can use inheritance and interfaces
public class AccountsLogic
{

    //Static properties are shared across all instances of the class
    //This can be used to get the current logged in account from anywhere in the program
    //private set, so this can only be set by the class itself
    public static AccountModel? CurrentAccount { get; private set; }

    public AccountsLogic()
    {

    }

    public UserModel? GetGuestDetails(AccountModel account)
    {
        if (CurrentAccount == null || CurrentAccount.Id != account.Id)
            return null;
        return AccountsAccess.GetUserByAccountId(CurrentAccount.Id);
    }

    public void Logout()
    {
        CurrentAccount = null;
    }

    public AccountModel? CheckLogin(string email, string password)
    {
        AccountModel? account = AccountsAccess.GetByEmail(email);
        if (account != null && account.Password == password)
        {
            CurrentAccount = account;
            return account;
        }

        CurrentAccount = null;
        return null;
    }

    public static List<string> Register(int userId, string password, string repeatedpassword, string role)
    {
        List<string> erros = new();

        if (!ValidateUser(userId))
            erros.Add("\n  User does not exist.");
        if (!ValidatePassword(password))
            erros.Add("\n  Invalid password.");
        if (password != repeatedpassword)
            erros.Add("\n  Passwords do not match.");
        if (string.IsNullOrWhiteSpace(role))
            erros.Add($"{role} is not a valid role.");

        if (erros.Count == 0)
        {
            AccountModel account = new(userId, password, role);
            AccountsAccess.Create(account);
        }
        return erros;

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
