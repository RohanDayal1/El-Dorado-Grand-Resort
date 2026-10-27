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

    public static UserRegisterErrors Register(string fullname, string email, string password, string repeatedpassword, string role)
    {
        
        
        if (!ValidatePassword(password))
            return UserRegisterErrors.InvalidPassword;
        if (password != repeatedpassword)
            return UserRegisterErrors.PasswordMismatch;
        if (string.IsNullOrWhiteSpace(role))
            return UserRegisterErrors.InvalidRole;
        
        AccountModel account = new(userId, password, role);

        
        
        AccountsAccess.Create(account);

        return UserRegisterErrors.Success;
    }

    public static UserModel CreateUser()
    {
        
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
